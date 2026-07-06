import axiosClient from "@/services/api/axiosClient";
import { ApiResponse } from "@/types/auth.types";
import {
  DepartmentDto,
  EmployeeInDepartmentDto,
  CreateDepartmentCommand,
  TransferEmployeeCommand,
  AdjustSalaryCommand,
  ChangePositionCommand,
} from "../types/department.types";

export const departmentApi = {
  getDepartments: () =>
    axiosClient.get<unknown, ApiResponse<DepartmentDto[]>>("/Departments"),
  createDepartment: (data: CreateDepartmentCommand) =>
    axiosClient.post<unknown, ApiResponse<string>>("/Departments", data),
  transferEmployee: (data: TransferEmployeeCommand) =>
    axiosClient.post<unknown, ApiResponse<boolean>>(
      "/Departments/transfer-employee",
      data,
    ),
  getEmployeesInDepartment: (idPb: string) =>
    axiosClient.get<unknown, ApiResponse<EmployeeInDepartmentDto[]>>(
      `/Departments/${idPb}/employees`,
    ),
  adjustSalary: (data: AdjustSalaryCommand) =>
    axiosClient.post<unknown, ApiResponse<boolean>>(
      "/Departments/adjust-salary",
      data,
    ),
  changePosition: (data: ChangePositionCommand) =>
    axiosClient.post<unknown, ApiResponse<boolean>>(
      "/Departments/change-position",
      data,
    ),
};
