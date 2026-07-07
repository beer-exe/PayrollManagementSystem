import axiosClient from '@/services/api/axiosClient';
import { ApiResponse } from '@/types/auth.types';
import { PositionDto, CreatePositionCommand, UpdatePositionCommand } from '../types/position.types';

export const positionApi = {
  getPositions: (params?: { searchTerm?: string; trangThai?: string; idPhongBan?: string }) => 
    axiosClient.get<unknown, ApiResponse<PositionDto[]>>('/Positions', { params }),
  createPosition: (data: CreatePositionCommand) => 
    axiosClient.post<unknown, ApiResponse<string>>('/Positions', data),
  updatePosition: (id: string, data: UpdatePositionCommand) => 
    axiosClient.put<unknown, ApiResponse<boolean>>(`/Positions/${id}`, data),
  toggleStatus: (id: string) => 
    axiosClient.put<unknown, ApiResponse<boolean>>(`/Positions/${id}/toggle-status`),
};