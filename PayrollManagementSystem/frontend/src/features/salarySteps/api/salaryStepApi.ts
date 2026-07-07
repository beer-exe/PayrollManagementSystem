import axiosClient from '@/services/api/axiosClient';
import { ApiResponse } from '@/types/auth.types';
import { SalaryStepDto, CreateSalaryStepCommand, UpdateSalaryStepVersionCommand } from '../types/salaryStep.types';

export const salaryStepApi = {
  getActive: (jobGradeId: string) => 
    axiosClient.get<unknown, ApiResponse<SalaryStepDto[]>>(`/SalarySteps/active/${jobGradeId}`),
    
  getHistory: (jobGradeId: string, stepName: string) => 
    axiosClient.get<unknown, ApiResponse<SalaryStepDto[]>>(`/SalarySteps/history/${jobGradeId}/${encodeURIComponent(stepName)}`),
    
  create: (data: CreateSalaryStepCommand) => 
    axiosClient.post<unknown, ApiResponse<string>>('/SalarySteps', data),
    
  updateVersion: (data: UpdateSalaryStepVersionCommand) => 
    axiosClient.post<unknown, ApiResponse<string>>('/SalarySteps/version', data),
    
  delete: (jobGradeId: string, stepName: string) => 
    axiosClient.delete<unknown, ApiResponse<boolean>>(`/SalarySteps/${jobGradeId}/${encodeURIComponent(stepName)}`),
};