import axiosClient from '@/services/api/axiosClient';
import { ApiResponse } from '@/types/auth.types';
import { JobGrade, CreateJobGradeDto, UpdateJobGradeDto } from '../types/jobGrade.types';

export const jobGradeApi = {
  getJobGrades: () => 
    axiosClient.get<unknown, ApiResponse<JobGrade[]>>('/JobGrades'),

  createJobGrade: (data: CreateJobGradeDto) => 
    axiosClient.post<unknown, ApiResponse<string>>('/JobGrades', data),

  updateJobGrade: (data: UpdateJobGradeDto) => 
    axiosClient.put<unknown, ApiResponse<boolean>>('/JobGrades', data),

  deleteJobGrade: (idNgachLuong: string) => 
    axiosClient.delete<unknown, ApiResponse<boolean>>(`/JobGrades/${idNgachLuong}`)
};
