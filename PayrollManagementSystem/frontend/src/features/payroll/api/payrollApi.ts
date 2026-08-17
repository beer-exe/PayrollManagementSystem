import axiosClient from '../../../services/api/axiosClient';
import { PayrollListDto, CalculatePayrollCommand, ReopenPayrollCommand, KyLuongStatusDto } from '../types/payroll.types';
import { ApiResponse } from '@/types/auth.types';

export const payrollApi = {
  getPayrollList: (thang: number, nam: number) => 
    axiosClient.get<unknown, ApiResponse<PayrollListDto[]>>('/Payroll', { params: { thang, nam } }),
    
  getKyLuongStatus: (thang: number, nam: number) => 
    axiosClient.get<unknown, ApiResponse<KyLuongStatusDto>>('/Payroll/status', { params: { thang, nam } }),

  calculatePayroll: (data: CalculatePayrollCommand) => 
    axiosClient.post<unknown, ApiResponse<boolean>>('/Payroll/calculate', data),

  closePayroll: (data: { thang: number; nam: number }) => 
    axiosClient.post<unknown, ApiResponse<boolean>>('/Payroll/close', data),

  reopenPayroll: (data: ReopenPayrollCommand) => 
    axiosClient.post<unknown, ApiResponse<boolean>>('/Payroll/reopen', data),
};
