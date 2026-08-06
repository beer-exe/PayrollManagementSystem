import axiosClient from '@/services/api/axiosClient';
import { ChamCongDto } from '../../chamCong/types/chamCong.types';

export const personalAttendanceApi = {
  getMyAttendance: async (thang: number, nam: number): Promise<ChamCongDto[]> => {
    const response = await axiosClient.get<ChamCongDto[]>(
      '/cham-cong/me',
      { params: { thang, nam } }
    );
    // axiosClient interceptor unwraps the Response<T>.data, returning the data directly.
    return response as any;
  }
};
