import axiosClient from '@/services/api/axiosClient';
import { ApiResponse } from '@/types/auth.types';
import { UserDto, RoleDto, CreateUserCommand, UpdateUserRoleCommand, ResetPasswordCommand } from '../types/user.types';

export const userApi = {
  getUsers: () => {
    return axiosClient.get<any, ApiResponse<UserDto[]>>('/Users');
  },
  getRoles: () => {
    return axiosClient.get<any, ApiResponse<RoleDto[]>>('/Users/roles');
  },
  createUser: (data: CreateUserCommand) => {
    return axiosClient.post<any, ApiResponse<string>>('/Users', data);
  },
  updateRole: (id: string, data: UpdateUserRoleCommand) => {
    return axiosClient.put<any, ApiResponse<boolean>>(`/Users/${id}/role`, data);
  },
  toggleStatus: (id: string) => {
    return axiosClient.put<any, ApiResponse<boolean>>(`/Users/${id}/toggle-status`);
  },
  resetPassword: (id: string, data: ResetPasswordCommand) => {
    return axiosClient.put<any, ApiResponse<boolean>>(`/Users/${id}/reset-password`, data);
  }
};