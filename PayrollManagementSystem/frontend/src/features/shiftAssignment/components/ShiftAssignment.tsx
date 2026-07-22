import React, { useState, useEffect } from 'react';
import './ShiftAssignment.css';
import { shiftAssignmentApi } from '../api/shiftAssignmentApi';
import { PhanCongCaDto } from '../types';
import { employeeApi } from '../../employees/api/employeeApi';
import { UserProfileDetail } from '../../../types/profile.types';
import { workShiftApi } from '../../workShifts/api/workShiftApi';
import { CaLamViec } from '../../workShifts/types';
import { departmentApi } from '../../departments/api/departmentApi';
import { DepartmentDto } from '../../departments/types/department.types';
import { workScheduleApi } from '../../workSchedule/api/workScheduleApi';
import { LichLamViecDto } from '../../workSchedule/types/workSchedule.types';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { Toast } from '../../../components/Toast/Toast';

const formatDateToYYYYMMDD = (date: Date) => {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
};

const formatDisplayDate = (date: Date) => {
    const days = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'];
    return `${days[date.getDay()]} - ${date.getDate()}/${date.getMonth() + 1}`;
};

const getShiftColor = (shiftId: string | null) => {
    if (!shiftId) {
        return {
            bg: '#fee2e2', // red-100
            border: '#fca5a5', // red-300
            text: '#991b1b' // red-800
        };
    }
    let hash = 0;
    for (let i = 0; i < shiftId.length; i++) {
        hash = shiftId.charCodeAt(i) + ((hash << 5) - hash);
    }
    const h = Math.abs(hash) % 360;
    return {
        bg: `hsl(${h}, 70%, 90%)`,
        border: `hsl(${h}, 70%, 75%)`,
        text: `hsl(${h}, 80%, 30%)`
    };
};

// ISO 8601 Week Calculation
const getWeeksInYear = (year: number) => {
    const weeks = [];
    let firstThursday = new Date(year, 0, 1);
    while (firstThursday.getDay() !== 4) {
        firstThursday.setDate(firstThursday.getDate() + 1);
    }
    
    let currentMonday = new Date(firstThursday);
    currentMonday.setDate(currentMonday.getDate() - 3);
    
    let weekNum = 1;
    
    while (true) {
        const weekStart = new Date(currentMonday);
        const weekEnd = new Date(currentMonday);
        weekEnd.setDate(weekEnd.getDate() + 6);
        
        const formatStr = (d: Date) => {
            const dd = String(d.getDate()).padStart(2, '0');
            const mm = String(d.getMonth() + 1).padStart(2, '0');
            const yyyy = d.getFullYear();
            return `${dd}/${mm}/${yyyy}`;
        };

        weeks.push({
            weekNumber: weekNum,
            startDate: new Date(weekStart),
            endDate: new Date(weekEnd),
            label: `Tuần ${weekNum} [từ ngày ${formatStr(weekStart)} đến ngày ${formatStr(weekEnd)}]`
        });
        
        currentMonday.setDate(currentMonday.getDate() + 7);
        const nextThursday = new Date(currentMonday);
        nextThursday.setDate(nextThursday.getDate() + 3);
        if (nextThursday.getFullYear() > year) break;
        weekNum++;
    }
    return weeks;
};

export const ShiftAssignment: React.FC = () => {
    // Basic state
    const [employees, setEmployees] = useState<UserProfileDetail[]>([]);
    const [shifts, setShifts] = useState<CaLamViec[]>([]);
    const [assignments, setAssignments] = useState<PhanCongCaDto[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);
    const [draggedShiftId, setDraggedShiftId] = useState<string | null>(null);

    // Filters Data
    const [schedules, setSchedules] = useState<LichLamViecDto[]>([]);
    const [departments, setDepartments] = useState<DepartmentDto[]>([]);
    const [weeks, setWeeks] = useState<any[]>([]);

    // Filters State
    const [selectedYear, setSelectedYear] = useState<number | ''>('');
    const [selectedMonth, setSelectedMonth] = useState<number | ''>('');
    const [selectedWeek, setSelectedWeek] = useState<number>(0);
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedDeptId, setSelectedDeptId] = useState<string>('');
    const [selectedEmpId, setSelectedEmpId] = useState<string>('');

    // Derived dates for the grid based on selectedWeek
    const [weekDates, setWeekDates] = useState<Date[]>([]);
    
    // Details of the work schedule for the current week's months
    const [scheduleDetails, setScheduleDetails] = useState<any[]>([]);

    useEffect(() => {
        const fetchFiltersData = async () => {
            try {
                const deptRes = await departmentApi.getDepartments();
                if (deptRes.succeeded) {
                    setDepartments(deptRes.data);
                }

                const schedRes = await workScheduleApi.getAll();
                if (schedRes.succeeded && schedRes.data.length > 0) {
                    setSchedules(schedRes.data);
                    const currentYear = new Date().getFullYear();
                    const hasCurrentYear = schedRes.data.find(s => s.nam === currentYear);
                    if (hasCurrentYear) {
                        setSelectedYear(currentYear);
                    } else {
                        // Max year
                        const maxYear = Math.max(...schedRes.data.map(s => s.nam));
                        setSelectedYear(maxYear);
                    }
                } else {
                    setSchedules([]);
                    setSelectedYear('');
                }
            } catch (err) {
                console.error("Lỗi khi tải dữ liệu bộ lọc", err);
            }
        };
        fetchFiltersData();
    }, []);

    useEffect(() => {
        if (selectedYear !== '') {
            const calculatedWeeks = getWeeksInYear(selectedYear);
            setWeeks(calculatedWeeks);
            
            // Auto select current week if it's the current year
            const today = new Date();
            if (selectedYear === today.getFullYear()) {
                const currentWeekObj = calculatedWeeks.find(w => today >= w.startDate && today <= w.endDate);
                if (currentWeekObj) {
                    setSelectedWeek(currentWeekObj.weekNumber);
                    setSelectedMonth(today.getMonth() + 1);
                } else {
                    setSelectedWeek(calculatedWeeks[0]?.weekNumber || 1);
                    setSelectedMonth('');
                }
            } else {
                setSelectedWeek(calculatedWeeks[0]?.weekNumber || 1);
                setSelectedMonth('');
            }
        } else {
            setWeeks([]);
            setSelectedWeek(0);
            setSelectedMonth('');
        }
    }, [selectedYear]);

    const filteredWeeks = React.useMemo(() => {
        return weeks.filter(w => {
            if (selectedMonth === '') return true;
            return (w.startDate.getMonth() + 1 === selectedMonth) || (w.endDate.getMonth() + 1 === selectedMonth);
        });
    }, [weeks, selectedMonth]);

    useEffect(() => {
        if (filteredWeeks.length > 0 && selectedWeek !== 0) {
            const hasCurrentWeek = filteredWeeks.some(w => w.weekNumber === selectedWeek);
            if (!hasCurrentWeek) {
                setSelectedWeek(filteredWeeks[0].weekNumber);
            }
        }
    }, [selectedMonth, weeks]);

    useEffect(() => {
        const fetchInitialData = async () => {
            try {
                const empRes = await employeeApi.getEmployees({ PageNumber: 1, PageSize: 1000 });
                if (empRes.succeeded && empRes.data) {
                    setEmployees(empRes.data.filter(e => e.trangThai === "DANG_LAM_VIEC")); 
                }
            } catch (err) {
                console.error("Failed to load employees", err);
            }

            try {
                const shiftRes = await workShiftApi.getAll();
                if (shiftRes.succeeded && shiftRes.data) {
                    setShifts(shiftRes.data.filter(s => s.trangThai)); 
                }
            } catch (err) {
                console.error("Failed to load shifts", err);
            }
        };
        fetchInitialData();
    }, []);

    useEffect(() => {
        if (selectedWeek && weeks.length > 0) {
            const currentWeek = weeks.find(w => w.weekNumber === selectedWeek);
            if (currentWeek) {
                const dates = [];
                for (let i = 0; i < 7; i++) {
                    const d = new Date(currentWeek.startDate);
                    d.setDate(d.getDate() + i);
                    dates.push(d);
                }
                setWeekDates(dates);
            }
        } else {
            setWeekDates([]);
        }
    }, [selectedWeek, weeks]);

    useEffect(() => {
        if (weekDates.length > 0) {
            loadAssignments();
        }
    }, [weekDates]);

    useEffect(() => {
        if (weekDates.length > 0 && schedules.length > 0) {
            loadScheduleDetails();
        }
    }, [weekDates, schedules, selectedYear]);

    const loadScheduleDetails = async () => {
        const idLich = schedules.find(s => s.nam === selectedYear)?.idLich;
        if (!idLich) return;

        const months = Array.from(new Set(weekDates.map(d => d.getMonth() + 1)));
        try {
            let details: any[] = [];
            for (const month of months) {
                const res = await workScheduleApi.getChiTiet(idLich, month, 1, 31);
                if (res.succeeded && res.data) {
                    details = [...details, ...res.data];
                }
            }
            setScheduleDetails(details);
        } catch (error) {
            console.error("Lỗi tải chi tiết lịch làm việc", error);
        }
    };

    const loadAssignments = async () => {
        setIsLoading(true);
        try {
            const startDate = formatDateToYYYYMMDD(weekDates[0]);
            const endDate = formatDateToYYYYMMDD(weekDates[6]);
            const res = await shiftAssignmentApi.getByDateRange(startDate, endDate);
            if (res.succeeded) {
                setAssignments(res.data);
            }
        } catch (error) {
            setToast({ message: "Lỗi khi tải dữ liệu phân công ca", type: "error" });
        } finally {
            setIsLoading(false);
        }
    };

    const handleExportExcel = () => {
        // We will implement export later when data is fully structured
        setToast({ message: "Chức năng xuất Excel sẽ được hoàn thiện sau", type: "success" });
    };

    const handleExportPdf = () => {
        setToast({ message: "Chức năng xuất PDF sẽ được hoàn thiện sau", type: "success" });
    };

    // Find selected department name for filtering
    const selectedDeptName = departments.find(d => d.idPb === selectedDeptId)?.tenPb;

    // Filter employees based on search, dept, emp select
    const filteredEmployees = employees.filter(emp => {
        if (searchTerm && !emp.hoTen.toLowerCase().includes(searchTerm.toLowerCase()) && !emp.cccd.includes(searchTerm)) {
            return false;
        }
        if (selectedDeptName && emp.tenPhongBan !== selectedDeptName) return false;
        if (selectedEmpId && emp.cccd !== selectedEmpId) return false;
        return true;
    });

    const handleDragStart = (e: React.DragEvent, shiftId: string) => {
        setDraggedShiftId(shiftId);
        e.dataTransfer.effectAllowed = 'copyMove';
    };

    const handleDragOver = (e: React.DragEvent) => {
        e.preventDefault(); 
        e.dataTransfer.dropEffect = 'copy';
        e.currentTarget.classList.add('drag-over');
    };

    const handleDragLeave = (e: React.DragEvent) => {
        e.currentTarget.classList.remove('drag-over');
    };

    const handleAssignShift = async (cccdNhanVien: string, date: Date, shiftId: string) => {
        const dateStr = formatDateToYYYYMMDD(date);
        const shiftObj = shifts.find(s => s.id === shiftId);
        const existingIndex = assignments.findIndex(a => a.cccdNhanVien === cccdNhanVien && a.ngayLamViec === dateStr);
        
        let newAssignments = [...assignments];
        if (existingIndex >= 0) {
            newAssignments[existingIndex] = { ...newAssignments[existingIndex], idCaLamViec: shiftId, tenCa: shiftObj?.tenCa || '' };
        } else {
            newAssignments.push({
                idPhanCong: 'temp',
                cccdNhanVien,
                ngayLamViec: dateStr,
                idCaLamViec: shiftId,
                tenCa: shiftObj?.tenCa || '',
                hoTenNhanVien: ''
            });
        }
        setAssignments(newAssignments);

        try {
            await shiftAssignmentApi.upsert({ cccdNhanVien, ngayLamViec: dateStr, idCaLamViec: shiftId, xoaPhanCong: false });
            const employee = employees.find(e => e.cccd === cccdNhanVien);
            setToast({ message: `Cập nhật phân công ca thành công cho nhân viên ${employee?.hoTen}`, type: "success" });
        } catch (error) {
            setToast({ message: "Lưu phân công thất bại", type: "error" });
            loadAssignments();
        }
    };

    const handleAssignOffDay = async (cccdNhanVien: string, date: Date) => {
        const dateStr = formatDateToYYYYMMDD(date);
        const existingIndex = assignments.findIndex(a => a.cccdNhanVien === cccdNhanVien && a.ngayLamViec === dateStr);
        
        let newAssignments = [...assignments];
        if (existingIndex >= 0) {
            newAssignments[existingIndex] = { ...newAssignments[existingIndex], idCaLamViec: null, tenCa: null };
        } else {
            newAssignments.push({
                idPhanCong: 'temp',
                cccdNhanVien,
                ngayLamViec: dateStr,
                idCaLamViec: null,
                tenCa: null,
                hoTenNhanVien: ''
            });
        }
        setAssignments(newAssignments);

        try {
            await shiftAssignmentApi.upsert({ cccdNhanVien, ngayLamViec: dateStr, idCaLamViec: null, xoaPhanCong: false });
            const employee = employees.find(e => e.cccd === cccdNhanVien);
            setToast({ message: `Gán ngày nghỉ đè ca mặc định thành công cho nhân viên ${employee?.hoTen}`, type: "success" });
        } catch (error) {
            setToast({ message: "Gán ngày nghỉ thất bại", type: "error" });
            loadAssignments();
        }
    };

    const handleDrop = async (e: React.DragEvent, cccdNhanVien: string, date: Date) => {
        e.preventDefault();
        e.currentTarget.classList.remove('drag-over');
        if (!draggedShiftId) return;
        
        if (draggedShiftId === 'OFF_DAY') {
            await handleAssignOffDay(cccdNhanVien, date);
        } else {
            await handleAssignShift(cccdNhanVien, date, draggedShiftId);
        }
        setDraggedShiftId(null);
    };

    const handleRemoveAssignment = async (cccdNhanVien: string, date: Date) => {
        const dateStr = formatDateToYYYYMMDD(date);
        setAssignments(prev => prev.filter(a => !(a.cccdNhanVien === cccdNhanVien && a.ngayLamViec === dateStr)));
        try {
            await shiftAssignmentApi.upsert({ cccdNhanVien, ngayLamViec: dateStr, idCaLamViec: null, xoaPhanCong: true });
            const employee = employees.find(e => e.cccd === cccdNhanVien);
            setToast({ message: `Xóa phân công ca (về mặc định) thành công cho nhân viên ${employee?.hoTen}`, type: "success" });
        } catch (error) {
            setToast({ message: "Xóa phân công thất bại", type: "error" });
            loadAssignments(); 
        }
    };

    return (
        <div className="sa-container">
            <div className="sa-header">
                <div className="sa-header-left">
                    <h2>Phân công ca luân phiên</h2>
                    <p className="sa-subtitle-desktop">Sắp xếp và quản lý ca làm việc bằng thao tác kéo thả (Drag & Drop)</p>
                    <p className="sa-subtitle-mobile">Chạm vào hộp chọn để gán ca làm việc cho nhân viên</p>
                </div>
            </div>

            <div className="sa-controls-wrapper">
                <div className="sa-filters">
                    <select
                        value={selectedYear}
                        onChange={(e) => setSelectedYear(e.target.value ? Number(e.target.value) : '')}
                        className="sa-select"
                        disabled={schedules.length === 0}
                    >
                        {schedules.length === 0 && <option value="">-- Chưa có lịch làm việc --</option>}
                        {schedules.map(s => (
                            <option key={s.idLich} value={s.nam}>Lịch làm việc {s.nam}</option>
                        ))}
                    </select>

                    <select
                        value={selectedMonth}
                        onChange={(e) => setSelectedMonth(e.target.value === '' ? '' : Number(e.target.value))}
                        className="sa-select"
                        disabled={weeks.length === 0}
                    >
                        <option value="">-- Tất cả các tháng --</option>
                        {Array.from({ length: 12 }, (_, i) => i + 1).map(m => (
                            <option key={m} value={m}>Tháng {m}</option>
                        ))}
                    </select>

                    <select
                        value={selectedWeek}
                        onChange={(e) => setSelectedWeek(Number(e.target.value))}
                        className="sa-select"
                        style={{ minWidth: '350px' }}
                        disabled={filteredWeeks.length === 0}
                    >
                        {filteredWeeks.length === 0 && <option value={0}>-- Không có tuần nào --</option>}
                        {filteredWeeks.map(w => (
                            <option key={w.weekNumber} value={w.weekNumber}>
                                {w.label}
                            </option>
                        ))}
                    </select>
                </div>
                
                <div className="sa-filters">
                    <div className="sa-input-wrapper">
                        <svg className="sa-input-icon" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.1rem', height: '1.1rem' }}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
                        </svg>
                        <input
                            type="text"
                            placeholder="Tìm nhân viên..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="sa-input"
                        />
                    </div>

                    <select
                        value={selectedDeptId}
                        onChange={(e) => setSelectedDeptId(e.target.value)}
                        className="sa-select"
                    >
                        <option value="">-- Tất cả phòng ban --</option>
                        {departments.map(d => (
                            <option key={d.idPb} value={d.idPb}>{d.tenPb}</option>
                        ))}
                    </select>

                    <select
                        value={selectedEmpId}
                        onChange={(e) => setSelectedEmpId(e.target.value)}
                        className="sa-select"
                    >
                        <option value="">-- Tất cả nhân viên --</option>
                        {employees.filter(e => !selectedDeptName || e.tenPhongBan === selectedDeptName).map(e => (
                            <option key={e.cccd} value={e.cccd}>{e.hoTen}</option>
                        ))}
                    </select>

                    <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
                </div>
                
                {schedules.length === 0 ? (
                    <div style={{ textAlign: 'center', padding: '3rem', marginTop: '1rem', background: 'var(--bg-hover)', borderRadius: '12px', border: '1px dashed var(--border-color)' }}>
                        <h3 style={{ color: 'var(--text-primary)', marginBottom: '0.5rem' }}>Chưa có dữ liệu Lịch làm việc</h3>
                        <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem' }}>Bạn cần tạo lịch làm việc trong chức năng "Lịch làm việc" trước khi có thể phân công ca.</p>
                    </div>
                ) : (
                    <div className="sa-filters" style={{ borderTop: '1px solid var(--border-color)', paddingTop: '1rem' }}>
                        <span className="sa-palette-label">Kéo thả ca:</span>
                        <div 
                            className="sa-shift-chip"
                            style={{ backgroundColor: '#fee2e2', borderColor: '#fca5a5', color: '#991b1b', fontWeight: 'bold' }}
                            draggable
                            onDragStart={(e) => handleDragStart(e, 'OFF_DAY')}
                        >
                            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" style={{ width: 16 }}>
                                <path fillRule="evenodd" d="M12 2.25c-5.385 0-9.75 4.365-9.75 9.75s4.365 9.75 9.75 9.75 9.75-4.365 9.75-9.75S17.385 2.25 12 2.25Zm-1.72 6.97a.75.75 0 1 0-1.06 1.06L10.94 12l-1.72 1.72a.75.75 0 1 0 1.06 1.06L12 13.06l1.72 1.72a.75.75 0 1 0 1.06-1.06L13.06 12l1.72-1.72a.75.75 0 1 0-1.06-1.06L12 10.94l-1.72-1.72Z" clipRule="evenodd" />
                            </svg>
                            Nghỉ
                        </div>
                        {shifts.map(s => {
                            const colors = getShiftColor(s.id);
                            return (
                                <div 
                                    key={s.id}
                                    className="sa-shift-chip"
                                    style={{ backgroundColor: colors.bg, borderColor: colors.border, color: colors.text }}
                                    draggable
                                    onDragStart={(e) => handleDragStart(e, s.id)}
                                >
                                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" style={{ width: 16 }}>
                                        <path fillRule="evenodd" d="M12 2.25c-5.385 0-9.75 4.365-9.75 9.75s4.365 9.75 9.75 9.75 9.75-4.365 9.75-9.75S17.385 2.25 12 2.25ZM12.75 6a.75.75 0 0 0-1.5 0v6c0 .414.336.75.75.75h4.5a.75.75 0 0 0 0-1.5h-3.75V6Z" clipRule="evenodd" />
                                    </svg>
                                    {s.tenCa}
                                </div>
                            )
                        })}
                    </div>
                )}
            </div>

            {/* View dành cho Desktop (Matrix Drag & Drop) */}
            {schedules.length > 0 && (
                <div className="sa-matrix-container" style={{ position: 'relative' }}>
                {isLoading && (
                    <div className="sa-loading-overlay">
                        Đang tải dữ liệu...
                    </div>
                )}
                <table className="sa-matrix-table">
                    <thead>
                        <tr>
                            <th>Nhân viên</th>
                            {weekDates.map((date, idx) => (
                                <th key={idx}>{formatDisplayDate(date)}</th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {filteredEmployees.map(emp => (
                            <tr key={emp.cccd}>
                                <td>
                                    <div className="sa-emp-info">
                                        <span className="sa-emp-name">{emp.hoTen}</span>
                                        <span className="sa-emp-dept">{emp.tenPhongBan || 'Chưa xếp phòng'}</span>
                                    </div>
                                </td>
                                {weekDates.map((date, idx) => {
                                    const dateStr = formatDateToYYYYMMDD(date);
                                    const assignment = assignments.find(a => a.cccdNhanVien === emp.cccd && a.ngayLamViec === dateStr);
                                    const defaultDetail = scheduleDetails.find(d => d.ngay.startsWith(dateStr));
                                    const hasDefaultShift = !assignment && defaultDetail?.idCaLamViecMacDinh;
                                    
                                    return (
                                        <td key={idx}>
                                            <div 
                                                className="sa-drop-cell"
                                                onDragOver={handleDragOver}
                                                onDragLeave={handleDragLeave}
                                                onDrop={(e) => handleDrop(e, emp.cccd, date)}
                                            >
                                                {assignment ? (
                                                    <div 
                                                        className="sa-assigned-shift"
                                                        style={{ 
                                                            backgroundColor: getShiftColor(assignment.idCaLamViec).bg, 
                                                            border: `1px solid ${getShiftColor(assignment.idCaLamViec).border}`,
                                                            color: getShiftColor(assignment.idCaLamViec).text
                                                        }}
                                                        draggable
                                                        onDragStart={(e) => handleDragStart(e, assignment.idCaLamViec || 'OFF_DAY')}
                                                    >
                                                        {assignment.idCaLamViec ? assignment.tenCa : "Ngày Nghỉ"}
                                                        <div 
                                                            className="sa-btn-remove-shift"
                                                            onClick={(e) => {
                                                                e.stopPropagation();
                                                                handleRemoveAssignment(emp.cccd, date);
                                                            }}
                                                        >
                                                            ✕
                                                        </div>
                                                    </div>
                                                ) : hasDefaultShift ? (
                                                    <div 
                                                        className="sa-assigned-shift"
                                                        style={{ 
                                                            backgroundColor: getShiftColor(defaultDetail.idCaLamViecMacDinh!).bg, 
                                                            border: `1px dashed ${getShiftColor(defaultDetail.idCaLamViecMacDinh!).border}`,
                                                            color: getShiftColor(defaultDetail.idCaLamViecMacDinh!).text,
                                                            opacity: 0.8
                                                        }}
                                                    >
                                                        {defaultDetail.tenCaLamViecMacDinh}
                                                    </div>
                                                ) : null}
                                            </div>
                                        </td>
                                    );
                                })}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            )}

            {/* View dành cho Mobile (Card List View) */}
            {schedules.length > 0 && (
                <div className="sa-mobile-container" style={{ position: 'relative' }}>
                    {isLoading && (
                        <div className="sa-loading-overlay">
                            Đang tải dữ liệu...
                        </div>
                    )}
                    {filteredEmployees.map(emp => (
                        <div className="sa-mobile-card" key={emp.cccd}>
                            <div className="sa-mobile-card-header">
                                <div className="sa-emp-info">
                                    <span className="sa-emp-name">{emp.hoTen}</span>
                                    <span className="sa-emp-dept">{emp.tenPhongBan || 'Chưa xếp phòng'}</span>
                                </div>
                            </div>
                            <div className="sa-mobile-card-body">
                                {weekDates.map((date, idx) => {
                                    const dateStr = formatDateToYYYYMMDD(date);
                                    const assignment = assignments.find(a => a.cccdNhanVien === emp.cccd && a.ngayLamViec === dateStr);
                                    const defaultDetail = scheduleDetails.find(d => d.ngay.startsWith(dateStr));
                                    
                                    return (
                                        <div className="sa-mobile-day-row" key={idx}>
                                            <div className="sa-mobile-day-label">
                                                {formatDisplayDate(date)}
                                            </div>
                                            <div className="sa-mobile-shift-select">
                                                <select 
                                                    className="sa-native-select"
                                                    value={assignment ? (assignment.idCaLamViec || 'OFF_DAY') : ''} 
                                                    onChange={(e) => {
                                                        const val = e.target.value;
                                                        if (val === '') {
                                                            handleRemoveAssignment(emp.cccd, date);
                                                        } else if (val === 'OFF_DAY') {
                                                            handleAssignOffDay(emp.cccd, date);
                                                        } else {
                                                            handleAssignShift(emp.cccd, date, val);
                                                        }
                                                    }}
                                                >
                                                    {defaultDetail?.idCaLamViecMacDinh ? (
                                                        <option value="">{defaultDetail.tenCaLamViecMacDinh} (Mặc định)</option>
                                                    ) : (
                                                        <option value="">-- Khôi phục về mặc định --</option>
                                                    )}
                                                    <option value="OFF_DAY" style={{ color: 'red', fontWeight: 'bold' }}>Nghỉ (Ghi đè)</option>
                                                    {shifts.map(s => (
                                                        <option key={s.id} value={s.id}>{s.tenCa}</option>
                                                    ))}
                                                </select>
                                            </div>
                                        </div>
                                    );
                                })}
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {toast && (
                <Toast
                    message={toast.message}
                    type={toast.type}
                    onClose={() => setToast(null)}
                />
            )}
        </div>
    );
};
