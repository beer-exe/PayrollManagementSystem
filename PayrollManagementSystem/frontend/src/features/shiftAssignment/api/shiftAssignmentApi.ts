import axiosClient from '../../../services/api/axiosClient';
import { ApiResponse } from '../../../types';
import { PhanCongCaDto, UpsertPhanCongCaCommand } from '../types';

export const shiftAssignmentApi = {
    getByDateRange: async (startDate: string, endDate: string, idPhongBan?: string) => {
        const response = await axiosClient.get('/PhanCongCa', {
            params: { startDate, endDate, idPhongBan }
        });
        return response as unknown as ApiResponse<PhanCongCaDto[]>;
    },

    upsert: async (command: UpsertPhanCongCaCommand) => {
        const response = await axiosClient.post('/PhanCongCa/upsert', command);
        return response as unknown as ApiResponse<boolean>;
    }
};
