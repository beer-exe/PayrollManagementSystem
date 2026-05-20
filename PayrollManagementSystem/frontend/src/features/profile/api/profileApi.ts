import axiosClient from '@/services/api/axiosClient';
import { UserProfileDetail } from '@/types/profile.types';
import { ApiResponse } from '@/types/auth.types';

export const profileApi = {
  getMyProfile: () => {
    return axiosClient.get<any, ApiResponse<UserProfileDetail>>('/Profile/me');
  },
};