export interface MucQuyDoiDto {
  idQuyDoi: string;
  xepLoai: string;
  diemToiThieu: number;
  diemToiDa: number;
  heSoP2: number;
}

export interface CreateMucQuyDoiDto {
  xepLoai: string;
  diemToiThieu: number;
  diemToiDa: number;
  heSoP2: number;
}
