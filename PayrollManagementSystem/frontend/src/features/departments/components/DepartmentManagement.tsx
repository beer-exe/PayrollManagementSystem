import React, { useState, useEffect } from "react";
import { Table, message, Empty, Dropdown, Button } from "antd";
import {
  MoreOutlined,
  SwapOutlined,
  RiseOutlined,
  IdcardOutlined,
} from "@ant-design/icons";
import { useSystemData } from "../hooks/useSystemData";
import { departmentApi } from "../api/departmentApi";
import { EmployeeInDepartmentDto } from "../types/department.types";

import { CreateDeptModal } from "./Modals/CreateDeptModal";
import { TransferModal } from "./Modals/TransferModal";
import { AdjustSalaryModal } from "./Modals/AdjustSalaryModal";
import { ChangePositionModal } from "./Modals/ChangePositionModal";

import { usePositions } from "@/features/positions/hooks/usePositions";
import "./DepartmentManagement.css";

export const DepartmentManagement: React.FC = () => {
  const { departments, isLoading, refreshData } = useSystemData();
  const { positions, fetchPositions } = usePositions();

  const [selectedDeptId, setSelectedDeptId] = useState<string | null>(null);
  const [deptEmployees, setDeptEmployees] = useState<EmployeeInDepartmentDto[]>(
    [],
  );
  const [loadingEmp, setLoadingEmp] = useState(false);

  const [isDeptModalOpen, setIsDeptModalOpen] = useState(false);
  const [isTransferModalOpen, setIsTransferModalOpen] = useState(false);
  const [isAdjustSalaryModalOpen, setIsAdjustSalaryModalOpen] = useState(false);
  const [isChangePositionModalOpen, setIsChangePositionModalOpen] =
    useState(false);

  const [selectedEmployee, setSelectedEmployee] =
    useState<EmployeeInDepartmentDto | null>(null);

  useEffect(() => {
    fetchPositions("", "HOAT_DONG");
  }, [fetchPositions]);

  useEffect(() => {
    if (selectedDeptId) {
      fetchEmployees(selectedDeptId);
    }
  }, [selectedDeptId]);

  const fetchEmployees = async (idPb: string) => {
    setLoadingEmp(true);
    try {
      const res = await departmentApi.getEmployeesInDepartment(idPb);
      if (res.succeeded) setDeptEmployees(res.data);
    } catch (error) {
      message.error("Lỗi tải danh sách nhân viên");
    } finally {
      setLoadingEmp(false);
    }
  };

  const handleOpenTransfer = (employee: EmployeeInDepartmentDto) => {
    setSelectedEmployee(employee);
    setIsTransferModalOpen(true);
  };

  const handleOpenAdjustSalary = (employee: EmployeeInDepartmentDto) => {
    setSelectedEmployee(employee);
    setIsAdjustSalaryModalOpen(true);
  };

  const handleOpenChangePosition = (employee: EmployeeInDepartmentDto) => {
    setSelectedEmployee(employee);
    setIsChangePositionModalOpen(true);
  };

  const columns = [
    {
      title: "Mã NV",
      dataIndex: "cccd",
      key: "cccd",
      className: "font-mono text-gray-500",
    },
    {
      title: "Họ tên",
      dataIndex: "hoTen",
      key: "hoTen",
      className: "font-semibold text-gray-900",
    },
    { title: "Chức vụ", dataIndex: "tenChucVu", key: "tenChucVu" },
    {
      title: "Trạng thái",
      dataIndex: "trangThai",
      key: "trangThai",
      render: (text: string, record: EmployeeInDepartmentDto) => (
        <span
          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
            text === "DANG_LAM_VIEC"
              ? "bg-emerald-100 text-emerald-700 border border-emerald-200"
              : "bg-gray-100 text-gray-600 border border-gray-200"
          }`}
        >
          {record.tenTrangThai || (text === "DANG_LAM_VIEC" ? "Đang làm việc" : "Đã nghỉ")}
        </span>
      ),
    },
    {
      title: "",
      key: "actions",
      width: 60,
      align: "center" as const,
      render: (_: unknown, record: EmployeeInDepartmentDto) =>
        record.trangThai === "DANG_LAM_VIEC" ? (
          <Dropdown
            menu={{
              items: [
                {
                  key: "change_position",
                  icon: <IdcardOutlined className="text-blue-600" />,
                  label: "Thay đổi chức vụ",
                  onClick: () => handleOpenChangePosition(record),
                },
                {
                  key: "adjust_salary",
                  icon: <RiseOutlined className="text-emerald-600" />,
                  label: "Điều chỉnh bậc lương",
                  onClick: () => handleOpenAdjustSalary(record),
                },
                {
                  type: "divider",
                },
                {
                  key: "transfer",
                  icon: <SwapOutlined className="text-violet-600" />,
                  label: "Điều chuyển phòng ban",
                  onClick: () => handleOpenTransfer(record),
                },
              ],
            }}
            trigger={["click"]}
            placement="bottomRight"
          >
            <Button
              type="text"
              icon={<MoreOutlined />}
              className="text-gray-500 hover:text-violet-600 hover:bg-violet-50"
            />
          </Dropdown>
        ) : null,
    },
  ];

  return (
    <div className="dept-wrapper">
      <div className="dept-header">
        <h2 className="dept-title">Phòng ban & Vị trí</h2>
        <div className="dept-actions">
          <button
            onClick={() => setIsDeptModalOpen(true)}
            className="dept-btn-primary"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth={2}
              stroke="currentColor"
              className="w-5 h-5"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M12 9v6m3-3H9m12 0a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
            Phòng ban
          </button>
        </div>
      </div>

      <div className="dept-content">
        <div className="dept-card dept-card-left">
          <div className="dept-card-header">
            Cơ cấu tổ chức
            <span className="bg-gray-100 text-gray-600 text-xs px-2 py-1 rounded-full">
              {departments.length}
            </span>
          </div>
          <div className="dept-list-body">
            {isLoading ? (
              <div className="dept-empty-state">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-violet-600"></div>
              </div>
            ) : departments.length === 0 ? (
              <div className="dept-empty-state">
                <Empty
                  description="Chưa có phòng ban"
                  image={Empty.PRESENTED_IMAGE_SIMPLE}
                />
              </div>
            ) : (
              departments.map((d) => (
                <button
                  key={d.idPb}
                  onClick={() => setSelectedDeptId(d.idPb)}
                  className={`dept-list-item ${selectedDeptId === d.idPb ? "active" : ""}`}
                >
                  <div>
                    <div className="dept-item-title">{d.tenPb}</div>
                    <div className="dept-item-subtitle font-mono">{d.idPb}</div>
                  </div>
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 20 20"
                    fill="currentColor"
                    className={`w-5 h-5 transition-transform ${selectedDeptId === d.idPb ? "text-violet-600 translate-x-1" : "text-gray-400"}`}
                  >
                    <path
                      fillRule="evenodd"
                      d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z"
                      clipRule="evenodd"
                    />
                  </svg>
                </button>
              ))
            )}
          </div>
        </div>

        <div className="dept-card dept-card-right">
          <div className="dept-card-header bg-white dark:bg-gray-800 border-b border-gray-100 dark:border-gray-700">
            <span className="flex items-center gap-2">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={1.5}
                stroke="currentColor"
                className="w-5 h-5 text-gray-500"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M15 19.128a9.38 9.38 0 002.625.372 9.337 9.337 0 004.121-.952 4.125 4.125 0 00-7.533-2.493M15 19.128v-.003c0-1.113-.285-2.16-.786-3.07M15 19.128v.106A12.318 12.318 0 018.624 21c-2.331 0-4.512-.645-6.374-1.766l-.001-.109a6.375 6.375 0 0111.964-3.07M12 6.375a3.375 3.375 0 11-6.75 0 3.375 3.375 0 016.75 0zm8.25 2.25a2.625 2.625 0 11-5.25 0 2.625 2.625 0 015.25 0z"
                />
              </svg>
              Danh sách nhân sự
              {selectedDeptId && (
                <span className="text-violet-600 ml-1">
                  / {departments.find((d) => d.idPb === selectedDeptId)?.tenPb}
                </span>
              )}
            </span>
          </div>
          <div className="dept-table-body">
            {!selectedDeptId ? (
              <div className="dept-empty-state">
                <Empty
                  description={
                    <span className="text-gray-500">
                      Vui lòng chọn phòng ban ở danh sách bên trái để xem chi
                      tiết nhân sự
                    </span>
                  }
                  image="https://gw.alipayobjects.com/zos/antfincdn/ZHrcdLPrvN/empty.svg"
                  imageStyle={{ height: 120 }}
                />
              </div>
            ) : (
              <Table
                dataSource={deptEmployees}
                columns={columns}
                rowKey="cccd"
                loading={loadingEmp}
                pagination={{
                  pageSize: 10,
                  position: ["bottomRight"],
                  className: "px-4",
                }}
                size="middle"
                scroll={{ x: "max-content" }}
                className="w-full"
                locale={{
                  emptyText: (
                    <Empty
                      description="Phòng ban này hiện chưa có nhân sự"
                      image={Empty.PRESENTED_IMAGE_SIMPLE}
                    />
                  ),
                }}
              />
            )}
          </div>
        </div>
      </div>

      {/* CÁC MODALS */}
      <CreateDeptModal
        isOpen={isDeptModalOpen}
        onClose={() => setIsDeptModalOpen(false)}
        onSuccess={refreshData}
      />

      <TransferModal
        isOpen={isTransferModalOpen}
        onClose={() => {
          setIsTransferModalOpen(false);
          setSelectedEmployee(null);
        }}
        onSuccess={() => {
          refreshData();
          if (selectedDeptId) fetchEmployees(selectedDeptId);
        }}
        departments={departments}
        positions={positions}
        employee={selectedEmployee}
      />

      <AdjustSalaryModal
        isOpen={isAdjustSalaryModalOpen}
        onClose={() => {
          setIsAdjustSalaryModalOpen(false);
          setSelectedEmployee(null);
        }}
        onSuccess={() => {
          refreshData();
          if (selectedDeptId) fetchEmployees(selectedDeptId);
        }}
        employee={selectedEmployee}
        positions={positions}
      />

      <ChangePositionModal
        isOpen={isChangePositionModalOpen}
        onClose={() => {
          setIsChangePositionModalOpen(false);
          setSelectedEmployee(null);
        }}
        onSuccess={() => {
          refreshData();
          if (selectedDeptId) fetchEmployees(selectedDeptId);
        }}
        employee={selectedEmployee}
        positions={positions.filter(p => p.idPhongBan === selectedDeptId)}
      />
    </div>
  );
};
