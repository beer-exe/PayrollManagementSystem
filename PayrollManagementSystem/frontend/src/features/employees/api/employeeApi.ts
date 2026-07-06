import axiosClient from '@/services/api/axiosClient';
import { UserProfileDetail } from '@/types/profile.types';
import { ApiResponse } from '@/types/auth.types';
import { ChangeStatusDto, CreateEmployeeCommand, UpdateEmployeeCommand } from '../types/employee.types';

export interface PagedResponse<T> extends ApiResponse<T> {
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalRecords: number;
}

export const employeeApi = {
  getEmployees: (params: { SearchTerm?: string, PageNumber: number, PageSize: number }) => {
    return axiosClient.get<unknown, PagedResponse<UserProfileDetail[]>>('/Employee', { params });
  },
  exportExcel: (params: { searchTerm?: string, idPb?: string }) => {
    return axiosClient.get('/Employee/export', { params, responseType: 'blob' });
  },
  createEmployee: (data: CreateEmployeeCommand) => {
    return axiosClient.post<unknown, ApiResponse<string>>('/Employee', data);
  },
  updateEmployee: (cccd: string, data: UpdateEmployeeCommand) => {
    return axiosClient.put<unknown, ApiResponse<boolean>>(`/Employee/${cccd}`, data);
  },
  changeStatus: (cccd: string, data: ChangeStatusDto) => {
    return axiosClient.put<unknown, ApiResponse<boolean>>(`/Employee/${cccd}/status`, data);
  },
  getRelations: () => {
    return axiosClient.get<unknown, ApiResponse<{ idMqh: string; tenQuanHe: string }[]>>('/Employee/relations');
  }
};