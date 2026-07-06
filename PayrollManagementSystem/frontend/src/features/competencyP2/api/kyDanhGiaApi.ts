import axiosClient from "@/services/api/axiosClient";
import { ApiResponse } from "@/types/auth.types";
import { KyDanhGiaDto, CreateKyDanhGiaDto } from "../types/kyDanhGia.types";

export const kyDanhGiaApi = {
  getAll: () => axiosClient.get<unknown, ApiResponse<KyDanhGiaDto[]>>('KyDanhGia'),
  create: (data: CreateKyDanhGiaDto) => axiosClient.post<unknown, ApiResponse<string>>('KyDanhGia', data),
  delete: (id: string) => axiosClient.delete<unknown, ApiResponse<boolean>>(`KyDanhGia/${id}`),
  changeStatus: (id: string, trangThaiMoi: number, force: boolean = false) => axiosClient.put<unknown, ApiResponse<boolean>>(`KyDanhGia/${id}/status`, { idKyDanhGia: id, trangThaiMoi, force }),
};
