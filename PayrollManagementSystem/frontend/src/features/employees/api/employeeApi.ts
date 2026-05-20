import axiosClient from '@/services/api/axiosClient';
import { UserProfileDetail } from '@/types/profile.types';
import { ApiResponse } from '@/types/auth.types';

export interface PagedResponse<T> extends ApiResponse<T> {
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalRecords: number;
}

export const employeeApi = {
  getEmployees: (params: { SearchTerm?: string, PageNumber: number, PageSize: number }) => {
    return axiosClient.get<any, PagedResponse<UserProfileDetail[]>>('/Employee', { params });
  },
};