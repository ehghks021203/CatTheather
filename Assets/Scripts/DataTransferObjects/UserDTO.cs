public class UserDTO : Singleton<UserDTO> {
    public bool IsLogin { get; set; }
    public string UserUID { get; set; }
    public int Fish { get; set; }
    public int Can { get; set; }
    public int Rep { get; set; }
}