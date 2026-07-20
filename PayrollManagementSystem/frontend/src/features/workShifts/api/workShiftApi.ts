import axiosClient from "@/services/api/axiosClient";
import { CaLamViec, CreateCaLamViecRequest, UpdateCaLamViecRequest } from "../types";

export interface ApiResponse<T> {
    succeeded: boolean;
    message: string;
    errors: string[] | null;
    data: T;
}

export const workShiftApi = {
    getAll: (trangThai?: boolean): Promise<ApiResponse<CaLamViec[]>> => {
        const params = trangThai !== undefined ? { trangThai } : {};
        return axiosClient.get("/CaLamViec", { params }) as unknown as Promise<ApiResponse<CaLamViec[]>>;
    },
    create: (data: CreateCaLamViecRequest): Promise<ApiResponse<string>> => {
        return axiosClient.post("/CaLamViec", data) as unknown as Promise<ApiResponse<string>>;
    },
    update: (id: string, data: UpdateCaLamViecRequest): Promise<ApiResponse<boolean>> => {
        return axiosClient.put(`/CaLamViec/${id}`, data) as unknown as Promise<ApiResponse<boolean>>;
    },
    delete: (id: string): Promise<ApiResponse<boolean>> => {
        return axiosClient.delete(`/CaLamViec/${id}`) as unknown as Promise<ApiResponse<boolean>>;
    }
};
