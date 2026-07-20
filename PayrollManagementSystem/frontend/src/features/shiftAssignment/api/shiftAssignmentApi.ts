import axiosClient from '../../../services/api/axiosClient';
import { ApiResponse } from '../../../types';
import { PhanCongCaDto, UpsertPhanCongCaCommand } from '../types';

export const shiftAssignmentApi = {
    getByDateRange: async (startDate: string, endDate: string, idPhongBan?: string) => {
        const response = await axiosClient.get<ApiResponse<PhanCongCaDto[]>>('/PhanCongCa', {
            params: { startDate, endDate, idPhongBan }
        });
        return response.data;
    },

    upsert: async (command: UpsertPhanCongCaCommand) => {
        const response = await axiosClient.post<ApiResponse<boolean>>('/PhanCongCa/upsert', command);
        return response.data;
    }
};
