import axiosClient from '../../../services/api/axiosClient';
import { MyScheduleDayDto } from '../types/personalSchedule.types';

export const personalScheduleApi = {
  getMySchedule: async (thang: number, nam: number) => {
    const response = await axiosClient.get<{ data: MyScheduleDayDto[], message: string }>(
      '/lich-lam-viec/me',
      { params: { thang, nam } }
    );
    return response;
  },
};
