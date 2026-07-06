import axiosClient from "@/services/api/axiosClient";
import { ApiResponse } from "@/types/auth.types";
import { MucQuyDoiDto, CreateMucQuyDoiDto } from "../types/mucQuyDoi.types";

export const mucQuyDoiApi = {
  getAll: () => axiosClient.get<unknown, ApiResponse<MucQuyDoiDto[]>>('MucQuyDoiP2'),
  create: (data: CreateMucQuyDoiDto) => axiosClient.post<unknown, ApiResponse<string>>('MucQuyDoiP2', data),
  update: (id: string, data: CreateMucQuyDoiDto) => axiosClient.put<unknown, ApiResponse<string>>(`MucQuyDoiP2/${id}`, data),
  delete: (id: string) => axiosClient.delete<unknown, ApiResponse<string>>(`MucQuyDoiP2/${id}`),
};
