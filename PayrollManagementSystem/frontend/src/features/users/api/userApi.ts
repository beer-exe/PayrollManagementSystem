import axiosClient from '@/services/api/axiosClient';
import { ApiResponse } from '@/types/auth.types';
import { UserDto, RoleDto, CreateUserCommand, UpdateUserRoleCommand, ResetPasswordCommand, EmployeeNoAccount } from '../types/user.types';

export const userApi = {
  getUsers: () => {
    return axiosClient.get<unknown, ApiResponse<UserDto[]>>('/Users');
  },
  getRoles: () => {
    return axiosClient.get<unknown, ApiResponse<RoleDto[]>>('/Users/roles');
  },
  createUser: (data: CreateUserCommand) => {
    return axiosClient.post<unknown, ApiResponse<string>>('/Users', data);
  },
  updateRole: (id: string, data: UpdateUserRoleCommand) => {
    return axiosClient.put<unknown, ApiResponse<boolean>>(`/Users/${id}/role`, data);
  },
  toggleStatus: (id: string) => {
    return axiosClient.put<unknown, ApiResponse<boolean>>(`/Users/${id}/toggle-status`);
  },
  resetPassword: (id: string, data: ResetPasswordCommand) => {
    return axiosClient.put<unknown, ApiResponse<boolean>>(`/Users/${id}/reset-password`, data);
  },
  getEmployeesNoAccount: () => {
    return axiosClient.get<unknown, ApiResponse<EmployeeNoAccount[]>>('/Users/employees-no-account');
  },
};