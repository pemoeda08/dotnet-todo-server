using AutoMapper;
using UserEntity = TodoServer.Data.Entities.User;


namespace TodoServer.Dto.User
{

    [AutoMap(typeof(UserEntity))]
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }
}