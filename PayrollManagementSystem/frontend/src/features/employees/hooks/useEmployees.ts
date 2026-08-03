import { useState, useCallback } from 'react';
import { employeeApi } from '../api/employeeApi';
import { ChangeStatusDto, CreateEmployeeCommand, UpdateEmployeeCommand } from '../types/employee.types';
import { UserProfileDetail } from '@/types/profile.types';

export const useEmployees = () => {
  const [employees, setEmployees] = useState<UserProfileDetail[]>([]);
  const [loading, setLoading] = useState(false);
  const [totalRecords, setTotalRecords] = useState(0);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

  const showToast = (message: string, type: "success" | "error") => {
    setToast({ message, type });
  };

  const fetchEmployees = useCallback(async (searchTerm: string, pageNumber: number, pageSize: number) => {
    setLoading(true);
    try {
      const response = await employeeApi.getEmployees({ SearchTerm: searchTerm, PageNumber: pageNumber, PageSize: pageSize });
      if (response.succeeded) {
        setEmployees(response.data);
        setTotalRecords(response.totalRecords);
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      showToast(error?.response?.data?.Message || 'Lỗi khi tải danh sách nhân viên', 'error');
    } finally {
      setLoading(false);
    }
  }, []);

  const [isExporting, setIsExporting] = useState(false);

  const exportExcel = async (searchTerm?: string) => {
    setIsExporting(true);
    try {
      const response = await employeeApi.exportExcel({ searchTerm });
      const url = window.URL.createObjectURL(new Blob([response as unknown as BlobPart]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `NhanVien_${new Date().getTime()}.xlsx`);
      document.body.appendChild(link);
      link.click();
      showToast('Xuất file Excel thành công', 'success');
    } catch (error) {
      showToast('Lỗi khi xuất file Excel', 'error');
    } finally {
      setIsExporting(false);
    }
  };

  const createEmployee = async (data: CreateEmployeeCommand): Promise<boolean> => {
    try {
      const res = await employeeApi.createEmployee(data);
      if (res.succeeded) {
        showToast('Thêm mới nhân viên thành công!', 'success');
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      showToast(error?.response?.data?.Message || 'Lỗi khi thêm mới nhân viên', 'error');
      return false;
    }
  };

  const updateEmployee = async (cccd: string, data: UpdateEmployeeCommand): Promise<boolean> => {
    try {
      const res = await employeeApi.updateEmployee(cccd, data);
      if (res.succeeded) {
        showToast('Cập nhật hồ sơ nhân viên thành công!', 'success');
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      showToast(error?.response?.data?.Message || 'Lỗi khi cập nhật hồ sơ', 'error');
      return false;
    }
  };

  const changeStatus = async (cccd: string, payload: ChangeStatusDto): Promise<boolean> => {
    try {
      const res = await employeeApi.changeStatus(cccd, payload);
      if (res.succeeded) {
        showToast('Cập nhật trạng thái thành công!', 'success');
        return true;
      }
      return false;
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      showToast(error?.response?.data?.Message || 'Lỗi khi cập nhật trạng thái', 'error');
      return false;
    }
  };

return { employees, totalRecords, loading, isExporting, fetchEmployees, exportExcel, createEmployee, changeStatus, updateEmployee, toast, setToast };
};