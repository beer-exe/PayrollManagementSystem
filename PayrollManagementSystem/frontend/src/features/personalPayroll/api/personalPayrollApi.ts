import axiosClient from '../../../services/api/axiosClient';
import { MyPayrollDto } from '../types/myPayroll.types';

export const personalPayrollApi = {
  getMyPayroll: async (nam: number): Promise<MyPayrollDto[]> => {
    const response = await axiosClient.get<any>('/MyPayroll', {
      params: { nam }
    });
    return response.data;
  },

  confirmPayslip: async (id: string): Promise<boolean> => {
    const response = await axiosClient.post<any>(`/Payroll/bang-luong/${id}/confirm`);
    return response.data;
  },

  requestReviewPayslip: async (id: string, lyDoKhieuNai: string): Promise<boolean> => {
    const response = await axiosClient.post<any>(`/Payroll/bang-luong/${id}/request-review`, {
      idBangLuong: id,
      lyDoKhieuNai
    });
    return response.data;
  }
};
