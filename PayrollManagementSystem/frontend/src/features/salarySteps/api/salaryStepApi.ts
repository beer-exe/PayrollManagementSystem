import axiosClient from '@/services/api/axiosClient';
import { ApiResponse } from '@/types/auth.types';
import { SalaryStepDto, CreateSalaryStepCommand, UpdateSalaryStepVersionCommand } from '../types/salaryStep.types';

export const salaryStepApi = {
  getActive: (positionId: string) => 
    axiosClient.get<unknown, ApiResponse<SalaryStepDto[]>>(`/SalarySteps/active/${positionId}`),
    
  getHistory: (positionId: string, stepName: string) => 
    axiosClient.get<unknown, ApiResponse<SalaryStepDto[]>>(`/SalarySteps/history/${positionId}/${encodeURIComponent(stepName)}`),
    
  create: (data: CreateSalaryStepCommand) => 
    axiosClient.post<unknown, ApiResponse<string>>('/SalarySteps', data),
    
  updateVersion: (data: UpdateSalaryStepVersionCommand) => 
    axiosClient.post<unknown, ApiResponse<string>>('/SalarySteps/version', data),
    
  delete: (positionId: string, stepName: string) => 
    axiosClient.delete<unknown, ApiResponse<boolean>>(`/SalarySteps/${positionId}/${encodeURIComponent(stepName)}`),
};