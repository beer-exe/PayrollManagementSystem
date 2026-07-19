import axiosClient from "@/services/api/axiosClient";
import { CaLamViec, CreateCaLamViecRequest, UpdateCaLamViecRequest } from "../types";

export interface ApiResponse<T> {
    succeeded: boolean;
    message: string;
    errors: string[] | null;
    data: T;
}

export const workShiftApi = {
    getAll: (trangThai?: boolean) => {
        const params = trangThai !== undefined ? { trangThai } : {};
        return axiosClient.get<ApiResponse<CaLamViec[]>>("/CaLamViec", { params });
    },
    create: (data: CreateCaLamViecRequest) => {
        return axiosClient.post<ApiResponse<string>>("/CaLamViec", data);
    },
    update: (id: string, data: UpdateCaLamViecRequest) => {
        return axiosClient.put<ApiResponse<boolean>>(`/CaLamViec/${id}`, data);
    },
    delete: (id: string) => {
        return axiosClient.delete<ApiResponse<boolean>>(`/CaLamViec/${id}`);
    }
};
