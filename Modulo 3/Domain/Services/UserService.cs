using Domain.Interfaces.Repositories;
using JWTAuthAuthentication3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
           _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetUserWithRole()
        {
            return await _userRepository.GetUserWithRole();
        }

        public async Task<User> GetById(int id)
        {
            return await _userRepository.GetOneBy(x=>x.Id == id);
        }
        public async Task<User> EditName(User user, string newName)
        {
            user.Name = newName;
            _userRepository.Update(user);
            await _userRepository.SaveChanges();
            return user;
        }

        public async Task DeleteUser(int id)
        {
            var user = await GetById(id);
            _userRepository.Remove(user);
            await _userRepository.SaveChanges();
        }

        public void Dispose()
        {
            _userRepository.Dispose();
        }

        public Task AddAsync(User entity)
        {

            return _userRepository.AddAsync(entity);
        }
    }
}
