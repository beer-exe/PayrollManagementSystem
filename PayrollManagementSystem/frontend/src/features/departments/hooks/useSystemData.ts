import { useState, useEffect } from 'react';
import { departmentApi } from '../api/departmentApi';
import { positionApi } from '../../positions/api/positionApi';
import { DepartmentDto } from '../types/department.types';
import { PositionDto } from '../../positions/types/position.types';

export const useSystemData = () => {
  const [departments, setDepartments] = useState<DepartmentDto[]>([]);
  const [positions, setPositions] = useState<PositionDto[]>([]);
  const [relations, setRelations] = useState<{idMqh: string; tenQuanHe: string}[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

  const showToast = (message: string, type: "success" | "error") => {
    setToast({ message, type });
  };

  const fetchSystemData = async () => {
    setIsLoading(true);
    try {
      const [deptRes, posRes, relRes] = await Promise.all([
        departmentApi.getDepartments(),
        positionApi.getPositions(),
        import('../../employees/api/employeeApi').then(m => m.employeeApi.getRelations())
      ]);
      if (deptRes.succeeded) setDepartments(deptRes.data);
      if (posRes.succeeded) setPositions(posRes.data);
      if (relRes.succeeded) setRelations(relRes.data);
    } catch (error) {
      console.error("Lỗi khi tải dữ liệu hệ thống", error);
      showToast('Lỗi khi tải dữ liệu hệ thống (Phòng ban/Chức vụ)', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => { fetchSystemData(); }, []);

  return { departments, positions, relations, isLoading, refreshData: fetchSystemData, toast, setToast };
};