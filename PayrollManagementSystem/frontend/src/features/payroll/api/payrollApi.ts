import axiosClient from '../../../services/api/axiosClient';
import { PayrollListDto, CalculatePayrollCommand } from '../types/payroll.types';
import { Response } from '../../../types/api';

export const payrollApi = {
  getPayrollList: (thang: number, nam: number) => 
    axiosClient.get('/Payroll', { params: { thang, nam } }) as unknown as Promise<Response<PayrollListDto[]>>,
    
  calculatePayroll: (data: CalculatePayrollCommand) => 
    axiosClient.post('/Payroll/calculate', data) as unknown as Promise<Response<boolean>>,
};
