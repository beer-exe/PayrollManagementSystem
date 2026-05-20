import { useState } from 'react';
import { authApi } from '../api/authApi';
import { useAuthStore } from '@/store/useAuthStore';
import { LoginRequestDto } from '@/types/auth.types';
import { AxiosError } from 'axios';

export const useLogin = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const loginAction = useAuthStore((state) => state.login);

  const login = async (credentials: LoginRequestDto) => {
    setIsLoading(true);
    setError(null);
    
    try {
      const response = await authApi.login(credentials);
      
      if (response.succeeded && response.data) {
        localStorage.setItem('accessToken', response.data.accessToken);
        localStorage.setItem('refreshToken', response.data.refreshToken);
        
        loginAction({
          id: response.data.userId,
          name: response.data.fullName,
          email: response.data.email,
        });
        
        return true;
      } else {
        setError((response as any).Message || response.message || 'Đăng nhập thất bại.');
        return false;
      }
    } catch (err) {
      const axiosError = err as AxiosError<any>;
      
      const errorData = axiosError.response?.data;
      
      const errorMessage = errorData?.Message || errorData?.message || 'Không thể kết nối đến máy chủ.';
      
      setError(errorMessage);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  return { login, isLoading, error };
};