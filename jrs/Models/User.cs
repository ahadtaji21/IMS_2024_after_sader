using System;

namespace jrs.Models{
    public class User{
        public Guid UserUid { get; set; }
        public string username { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
        public string localCode { get; set; }

        public User(){ }
        public User(ImsUser user){
            if(user != null){
                this.UserUid = user.ImsUserUid;
                this.username = user.ImsUserUsername;
                this.localCode = user.ImsUserLocale;
            }
        }
        public User(ImsUser user, string token) : this(user){
            this.token = token;
        }
        public User(ImsUser user, string token, string refreshToken) : this(user, token){
            this.refreshToken = refreshToken;
        }
    }
}