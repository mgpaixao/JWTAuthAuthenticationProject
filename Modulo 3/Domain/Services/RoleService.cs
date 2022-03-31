using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using JWTAuthAuthentication3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        public RoleService(IRoleRepository roleRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }
        public Task AddAsync(Role entity)
        {
            return _roleRepository.AddAsync(entity);
        }

        public void Dispose()
        {
            _roleRepository.Dispose();
        }

        public async Task<bool> Exists(Expression<Func<Role, bool>> expression)
        {
            return await _roleRepository.Exists(expression);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role> GetById(Guid id)
        {
            return await _roleRepository.GetById(id);
        }

        public async Task<string> ChangeUserRole(int userId, int newRoleId, string? userClaim)
        {
            //Consultas
            var user = await _userRepository.GetOneBy(x=>x.Id == userId);
            var role = await _roleRepository.GetOneBy(x => x.Id == newRoleId);

            //Validações
            if (user == null || role == null)
                return "invalid Id";

            //Verificando quantos usuarios admins temos no sistema
            var userList = await _userRepository.GetAllAsync();
            int adminUsers = 0;
            foreach (var item in userList)
            {
                if (item.RolesId == 2) adminUsers++;
            }

            //Verificando se sobrará pelo menos 1 admin no sistema
            if (user.RolesId == 2 && adminUsers <= 1) return "You must have at least 1 admin user in the database";

            //Admin não pode mudar sua própria role
            if (userClaim.Equals(user.Id.ToString())) return "You cannot change your own role";
            
            user.Roles = role;

            await _roleRepository.SaveChanges();

            return $"{user.Name} Role changed to {role.Name}";

        }


        public async Task<string> EditRole(Role roleRequest)
        {
            var role = await _roleRepository.GetOneBy(x => x.Id == roleRequest.Id);
            if (role == null) return "Role not found";

            if (await _roleRepository.Exists(x => x.Name.Equals(roleRequest.Name))) return "Role duplicated";

            _roleRepository.Update(role);
            await _roleRepository.SaveChanges();
            return roleRequest.Name;
        }

        public async Task<string> DeleteRole(int roleId)
        {
            var role = await _roleRepository.GetOneBy(x => x.Id == roleId);
            
            if (role == null) return "Role not found";
            if (role.Name.Contains("admin")) return "Cannot delete admin role";

            Remove(role);
            await _roleRepository.SaveChanges();

            return "Role Deleted";

        }

        public void Remove(Role entity)
        {
            _roleRepository.Remove(entity);
        }

        public void Update(Role entity)
        {
            _roleRepository.Update(entity); 
        }
    }
}
