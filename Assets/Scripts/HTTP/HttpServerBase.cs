using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class HttpServerBase : MonoBehaviour {
    // 통신 결과를 담기위한 클래스
    public class Result {
        private string json;            // 서버 응답 값 (json)
        private bool isSuccess;         // 성공 여부
        private bool isNetworkError;    // 네트워크 에러 체크
        private int networkErrorCount;  // 네트워크 에러 발생 횟수 체크
        private string errorMsg;        // 에러 문구

        public string Json => json;
        public bool IsSuccess => isSuccess;
        public bool IsNetworkError => isNetworkError;
        public int NetworkErrorCount => networkErrorCount;
        public string Error => errorMsg;

        public Result(string json, bool isSuccess, bool isNetworkError, string errorMsg) {
            this.json = json;
            this.isSuccess = isSuccess;
            this.isNetworkError = isNetworkError;
            this.networkErrorCount = 0;
            this.errorMsg = errorMsg;
        }

        public void NetworkErrorCheck() {
            this.networkErrorCount += 1;
        }
    }

    protected virtual IEnumerator SendRequest(string url, string sendType, JObject json, Action<Result> onSucceed, Action<Result> onFailed) {
        // 네트워크 연결 상태 체크
        yield return StartCoroutine(CheckNetwork());

        var req = new UnityWebRequest(url, sendType);
        // Body
        byte[] jsonRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json));
        req.uploadHandler = new UploadHandlerRaw(jsonRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        // Header
        req.SetRequestHeader("Content-Type", "application/json");

        // 전송
        yield return req.SendWebRequest();

        // 성공인지 실채인지 확인
        var result = ResultCheck(req);
        
        if (result.IsNetworkError) {
            // 네트워크 에러
            Debug.LogError("네트워크 오류");
            yield return new WaitForSeconds(1f);
            Debug.LogError("재시도");
            result.NetworkErrorCheck();
            if (result.NetworkErrorCount > 6) {
                // TODO: 싱글톤으로 만든 팝업창을 띄우는 함수 작성하기
            } else {
                yield return StartCoroutine(SendRequest(url, sendType, json, onSucceed, onFailed));
            }
        } else {
            // 통신 성공
            if (result.IsSuccess) {
                onSucceed?.Invoke(result);
            } else {
                onFailed?.Invoke(result);
            }
        }
    }

    protected virtual IEnumerator CheckNetwork() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            Debug.LogError("네트워크 연결 안됨");
            yield return new WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);
            Debug.Log("네트워크 재연결됨");
        }
    }

    protected virtual Result ResultCheck(UnityWebRequest req) {
        Result res;
        switch (req.result) {
            case UnityWebRequest.Result.InProgress:
                res = new Result(req.downloadHandler.text, false, true, "InProgress");
                return res;
            case UnityWebRequest.Result.Success:
                JObject json = JObject.Parse(req.downloadHandler.text);
                bool isSuccess = json["result"].ToString() == "success" ? true : false;
                
                if (isSuccess) {
                    // 성공
                    res = new Result(req.downloadHandler.text, true, false, string.Empty);
                    return res;
                } else {
                    // 실패
                    Debug.LogErrorFormat("요청 실패: {0}", json["message"].ToString());
                    res = new Result(req.downloadHandler.text, false, false, json["message"].ToString());
                    return res;
                }
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                // 통신에러
                Debug.LogError(req.error);
                Debug.Log(req.downloadHandler.text);
                res = new Result(req.downloadHandler.text, false, true, req.error);
                return res;
            default:
                Debug.LogError("디폴트 케이스에 걸림");
                Debug.LogError(req.error);
                Debug.Log(req.downloadHandler.text);
                res = new Result(req.downloadHandler.text, false, true, "Unknown");
                return res;
        }
    }
}
