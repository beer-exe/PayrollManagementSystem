import axiosClient from "@/services/api/axiosClient";
import { ApiResponse } from "@/types/auth.types";
import { 
  PhieuDanhGiaDto, 
  GenerateMyPhieuDanhGiaCommand, 
  SubmitTuDanhGiaCommand,
  SubmitManagerEvaluationCommand
} from "../types/phieuDanhGia.types";

export const phieuDanhGiaApi = {
  getMyEvaluations: () => 
    axiosClient.get<unknown, ApiResponse<PhieuDanhGiaDto[]>>('/PhieuDanhGia/my-evaluations'),
  
  getById: (id: string) => 
    axiosClient.get<unknown, ApiResponse<PhieuDanhGiaDto>>(`/PhieuDanhGia/${id}`),
    
  generate: (data: GenerateMyPhieuDanhGiaCommand) => 
    axiosClient.post<unknown, ApiResponse<string>>('/PhieuDanhGia/generate', data),
    
  submitTuDanhGia: (data: SubmitTuDanhGiaCommand) => 
    axiosClient.post<unknown, ApiResponse<boolean>>('/PhieuDanhGia/submit', data),

  getManagerEvaluations: () => 
    axiosClient.get<unknown, ApiResponse<PhieuDanhGiaDto[]>>('/PhieuDanhGia/manager-evaluations'),

  managerSubmit: (data: SubmitManagerEvaluationCommand) => 
    axiosClient.post<unknown, ApiResponse<boolean>>('/PhieuDanhGia/manager-submit', data),
};
