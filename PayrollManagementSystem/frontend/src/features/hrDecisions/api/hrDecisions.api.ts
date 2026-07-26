import axiosClient from '../../../services/api/axiosClient';
import { ApiResponse } from '../../../types/common.types';

export const hrDecisionsApi = {
    generateCode: async (type: string): Promise<ApiResponse<string>> => {
        const response = await axiosClient.get(`/QuyetDinhNhanSu/generate-code`, {
            params: { type }
        });
        return response.data;
    }
};
