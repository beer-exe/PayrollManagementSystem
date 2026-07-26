import axiosClient from '@/services/api/axiosClient';
import { UserProfileDetail } from '@/types/profile.types';
import { ApiResponse } from '@/types/auth.types';

export const profileApi = {
  getMyProfile: () => {
    return axiosClient.get<unknown, ApiResponse<UserProfileDetail>>('/Profile/me');
  },
  changePassword: (data: any) => {
    return axiosClient.put<unknown, ApiResponse<boolean>>('/Profile/me/change-password', data);
  },
  updateAvatar: (avatarBase64: string) => {
    return axiosClient.put<unknown, ApiResponse<string>>('/Profile/me/avatar', { avatarBase64 });
  }
};