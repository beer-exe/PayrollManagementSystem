import React, { useState, useMemo } from 'react';
import { Switch, Modal, Table, Dropdown, MenuProps, Empty } from 'antd';
import { useUsers } from '../hooks/useUsers';
import { CreateUserModal } from './CreateUserModal';
import { UpdateRoleModal } from './UpdateRoleModal';
import { UserDto } from '../types/user.types';
import './UserManagement.css';

export const UserManagement: React.FC = () => {
  const { users, roles, isLoading, handleCreateUser, handleUpdateRole, handleToggleStatus, handleResetPassword } = useUsers();
  
  const [searchTerm, setSearchTerm] = useState('');
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [roleModalUser, setRoleModalUser] = useState<UserDto | null>(null);

  const filteredUsers = useMemo(() => {
    if (!searchTerm) return users;
    const lower = searchTerm.toLowerCase();
    return users.filter(u => 
      u.tenTaiKhoan.toLowerCase().includes(lower) || 
      (u.hoTen && u.hoTen.toLowerCase().includes(lower)) ||
      (u.email && u.email.toLowerCase().includes(lower))
    );
  }, [users, searchTerm]);

  const onResetPasswordClick = (user: UserDto) => {
    Modal.confirm({
      title: 'Xác nhận đặt lại mật khẩu',
      content: `Hệ thống sẽ đặt lại mật khẩu cho tài khoản ${user.tenTaiKhoan} và gửi email thông báo.`,
      okText: 'Xác nhận',
      cancelText: 'Hủy',
      okButtonProps: { className: 'bg-red-600 hover:bg-red-700 border-none rounded-lg' },
      cancelButtonProps: { className: 'rounded-lg' },
      onOk: async () => {
        await handleResetPassword(user.idTaiKhoan, { idTaiKhoan: user.idTaiKhoan, newPassword: 'NewPassword@123' });
      }
    });
  };

  const getRoleBadgeClass = (roleName: string) => {
    const lower = roleName.toLowerCase();
    if (lower.includes('admin') || lower.includes('quản trị')) return 'role-admin';
    if (lower.includes('hr') || lower.includes('nhân sự')) return 'role-hr';
    return 'role-user';
  };

  const columns = [
    {
      title: 'Tài khoản',
      dataIndex: 'tenTaiKhoan',
      key: 'tenTaiKhoan',
      render: (text: string) => <span className="font-bold text-gray-900 dark:text-white">{text}</span>
    },
    {
      title: 'Thông tin nhân viên',
      key: 'info',
      render: (_: unknown, record: UserDto) => (
        <div className="flex flex-col">
          <span className="font-medium text-gray-800 dark:text-gray-200">{record.hoTen}</span>
          <span className="text-xs text-gray-500">{record.email || 'Chưa cập nhật email'}</span>
        </div>
      )
    },
    {
      title: 'Vai trò',
      dataIndex: 'tenVaiTro',
      key: 'tenVaiTro',
      render: (role: string) => (
        <span className={`user-badge ${getRoleBadgeClass(role)}`}>
          {role}
        </span>
      )
    },
    {
      title: 'Trạng thái',
      key: 'trangThai',
      align: 'center' as const,
      render: (_: unknown, record: UserDto) => {
        const isActive = record.trangThai === 'HOAT_DONG';
        return (
          <div className="flex items-center justify-center gap-2">
            <Switch 
              size="small"
              checked={isActive} 
              onChange={() => handleToggleStatus(record.idTaiKhoan)}
              className={isActive ? 'bg-emerald-500' : 'bg-gray-400'}
            />
            <span className={`text-xs font-medium w-16 text-left ${isActive ? 'text-emerald-600' : 'text-gray-500'}`}>
              {isActive ? 'Hoạt động' : 'Đã khóa'}
            </span>
          </div>
        );
      }
    },
    {
      title: 'Hành động',
      key: 'actions',
      align: 'right' as const,
      render: (_: unknown, record: UserDto) => {
        const items: MenuProps['items'] = [
          {
            key: 'edit_role',
            label: 'Đổi quyền hạn',
            icon: <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-4 h-4"><path strokeLinecap="round" strokeLinejoin="round" d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z" /></svg>,
            onClick: () => setRoleModalUser(record)
          },
          {
            type: 'divider',
          },
          {
            key: 'reset_pass',
            label: 'Đặt lại mật khẩu',
            icon: <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-4 h-4"><path strokeLinecap="round" strokeLinejoin="round" d="M15.75 5.25a3 3 0 013 3m3 0a6 6 0 01-7.029 5.912c-.563-.097-1.159.026-1.563.43L10.5 17.25H8.25v2.25H6v2.25H2.25v-2.818c0-.597.237-1.17.659-1.591l6.499-6.499c.404-.404.527-1 .43-1.563A6 6 0 1121.75 8.25z" /></svg>,
            danger: true,
            onClick: () => onResetPasswordClick(record)
          }
        ];

        return (
          <Dropdown menu={{ items }} trigger={['click']} placement="bottomRight">
            <button className="p-1.5 text-gray-500 hover:text-violet-600 hover:bg-violet-50 rounded-md transition-colors dark:hover:bg-gray-700 focus:outline-none">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5"><path strokeLinecap="round" strokeLinejoin="round" d="M12 6.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 12.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 18.75a.75.75 0 110-1.5.75.75 0 010 1.5z" /></svg>
            </button>
          </Dropdown>
        );
      }
    }
  ];

  return (
    <div className="user-wrapper">
      <div className="user-header">
        <div>
          <h2 className="user-title">Quản lý Tài khoản</h2>
          <p className="text-sm text-gray-500 mt-1">Phân quyền và kiểm soát truy cập hệ thống</p>
        </div>
        <button onClick={() => setIsCreateOpen(true)} className="user-btn-primary">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5"><path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" /></svg>
          Thêm tài khoản
        </button>
      </div>

      <div className="user-card">
        {/* Toolbar */}
        <div className="user-toolbar">
          <div className="user-search-box">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5 text-gray-400"><path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" /></svg>
            <input 
              type="text" 
              placeholder="Tìm theo username, họ tên, email..." 
              className="user-search-input"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        {/* Data Table */}
        <div className="flex-1 overflow-auto bg-white dark:bg-gray-800">
          <Table 
            dataSource={filteredUsers} 
            columns={columns} 
            rowKey="idTaiKhoan"
            loading={isLoading}
            pagination={{ 
              pageSize: 10, 
              className: 'px-4 py-3 m-0 border-t border-gray-100 dark:border-gray-700 bg-gray-50 dark:bg-gray-800/50',
              showTotal: (total) => <span className="text-gray-500 font-medium">Tổng số {total} tài khoản</span>
            }}
            locale={{ emptyText: <Empty description="Không tìm thấy dữ liệu" image={Empty.PRESENTED_IMAGE_SIMPLE} /> }}
            scroll={{ x: 'max-content' }}
          />
        </div>
      </div>

      {/* Render Modals */}
      <CreateUserModal 
        isOpen={isCreateOpen} 
        onClose={() => setIsCreateOpen(false)} 
        onSubmit={handleCreateUser} 
        roles={roles}
      />
      <UpdateRoleModal 
        user={roleModalUser}
        isOpen={!!roleModalUser}
        onClose={() => setRoleModalUser(null)}
        onSubmit={handleUpdateRole}
        roles={roles}
      />
    </div>
  );
};