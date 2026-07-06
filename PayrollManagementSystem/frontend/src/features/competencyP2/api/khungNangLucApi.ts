import axiosClient from "@/services/api/axiosClient";
import { ApiResponse } from "@/types/auth.types";
import { KhungNangLucDto, CreateKhungNangLucCommand, UpdateKhungNangLucCommand } from "../types/khungNangLuc.types";

export const khungNangLucApi = {
  getByChucVu: (idChucVu: string) => 
    axiosClient.get<unknown, ApiResponse<KhungNangLucDto[]>>(`/KhungNangLuc/${idChucVu}`),
    
  create: (data: CreateKhungNangLucCommand) => 
    axiosClient.post<unknown, ApiResponse<string>>('/KhungNangLuc', data),
    
  update: (id: string, data: UpdateKhungNangLucCommand) => 
    axiosClient.put<unknown, ApiResponse<boolean>>(`/KhungNangLuc/${id}`, data),
    
  delete: (id: string) => 
    axiosClient.delete<unknown, ApiResponse<boolean>>(`/KhungNangLuc/${id}`),
};
