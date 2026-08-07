import axiosClient from '../../../services/api/axiosClient';
import { MyPayrollDto } from '../types/myPayroll.types';

export const personalPayrollApi = {
  getMyPayroll: async (nam: number): Promise<MyPayrollDto[]> => {
    const response = await axiosClient.get<any>('/MyPayroll', {
      params: { nam }
    });
    return response.data;
  }
};
