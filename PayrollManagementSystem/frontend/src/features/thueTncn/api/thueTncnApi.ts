import axiosClient from "@/services/api/axiosClient";
import {
  BacThueDto,
  CauHinhGiamTruDto,
  CreateBacThueRequest,
  UpdateBacThueRequest,
} from "../types/thueTncn.types";

const BASE = "/thue-tncn";

export const thueTncnApi = {
  getBacThueList: () =>
    axiosClient.get<{ data: BacThueDto[] }>(`${BASE}/bac-thue`).then((r) => r.data),

  createBacThue: (data: CreateBacThueRequest) =>
    axiosClient.post(`${BASE}/bac-thue`, data).then((r) => r.data),

  updateBacThue: (id: string, data: UpdateBacThueRequest) =>
    axiosClient.put(`${BASE}/bac-thue/${id}`, data).then((r) => r.data),

  deleteBacThue: (id: string) =>
    axiosClient.delete(`${BASE}/bac-thue/${id}`).then((r) => r.data),

  getCauHinhGiamTru: () =>
    axiosClient.get<{ data: CauHinhGiamTruDto }>(`${BASE}/giam-tru`).then((r) => r.data),

  upsertCauHinhGiamTru: (data: Omit<CauHinhGiamTruDto, "idCauHinhGiamTru">) =>
    axiosClient.put(`${BASE}/giam-tru`, data).then((r) => r.data),
};
