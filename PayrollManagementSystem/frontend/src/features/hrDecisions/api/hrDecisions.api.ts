import axiosClient from '../../../services/api/axiosClient';
import { ApiResponse } from '../../../types/common.types';

export const hrDecisionsApi = {
    generateCode: (type: string) => 
        axiosClient.get<unknown, ApiResponse<string>>(`/QuyetDinhNhanSu/generate-code`, {
            params: { type }
        })
};
