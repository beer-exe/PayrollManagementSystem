import axiosClient from '@/services/api/axiosClient';
import { LoginRequestDto, AuthResponseDto, ApiResponse } from '@/types/auth.types';

export const authApi = {
  login: (data: LoginRequestDto) => {
    return axiosClient.post('/Auth/login', data) as Promise<ApiResponse<AuthResponseDto>>;
  },
};