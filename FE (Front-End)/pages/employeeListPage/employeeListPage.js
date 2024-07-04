const employees = [
    {
        maNhanVien: 'nv-0001',
        hoVaTen: 'Hoàng Thị Lan',
        gioiTinh: 'Nữ',
        ngaySinh: '07/01/1978',
        email: 'hoàng1@example.com',
        diaChi: '78 Đội Cấn'
    },
    {
        maNhanVien: 'nv-0002',
        hoVaTen: 'Hoàng Văn Sơn',
        gioiTinh: 'Nữ',
        ngaySinh: '24/09/1993',
        email: 'hoàng2@example.com',
        diaChi: '177 Đội Cấn'
    },
    {
        maNhanVien: 'nv-0003',
        hoVaTen: 'Bùi Thị Lan',
        gioiTinh: 'Nữ',
        ngaySinh: '08/10/1999',
        email: 'bùi3@example.com',
        diaChi: '90 Tôn Thất Thuyết'
    },
    {
        maNhanVien: 'nv-0004',
        hoVaTen: 'Phan Văn Bình',
        gioiTinh: 'Nữ',
        ngaySinh: '04/07/1990',
        email: 'phan4@example.com',
        diaChi: '172 Tôn Thất Thuyết'
    },
    {
        maNhanVien: 'nv-0005',
        hoVaTen: 'Phạm Văn Sơn',
        gioiTinh: 'Nam',
        ngaySinh: '25/06/1991',
        email: 'phạm5@example.com',
        diaChi: '177 Phùng Khoang'
    },
    {
        maNhanVien: 'nv-0006',
        hoVaTen: 'Đỗ Văn Bình',
        gioiTinh: 'Nam',
        ngaySinh: '30/05/1985',
        email: 'đỗ6@example.com',
        diaChi: '85 Kim Mã'
    },
    {
        maNhanVien: 'nv-0007',
        hoVaTen: 'Bùi Văn Sơn',
        gioiTinh: 'Nữ',
        ngaySinh: '11/03/1978',
        email: 'bùi7@example.com',
        diaChi: '124 Xuân Thủy'
    },
    {
        maNhanVien: 'nv-0008',
        hoVaTen: 'Lê Văn Hùng',
        gioiTinh: 'Nam',
        ngaySinh: '21/09/1986',
        email: 'lê8@example.com',
        diaChi: '137 Kim Mã'
    },
    {
        maNhanVien: 'nv-0009',
        hoVaTen: 'Phan Văn Bình',
        gioiTinh: 'Nữ',
        ngaySinh: '07/04/1975',
        email: 'phan9@example.com',
        diaChi: '56 Trần Duy Hưng'
    },
    {
        maNhanVien: 'nv-0010',
        hoVaTen: 'Đỗ Văn Dũng',
        gioiTinh: 'Nữ',
        ngaySinh: '09/04/1975',
        email: 'đỗ10@example.com',
        diaChi: '27 Phùng Khoang'
    },
    {
        maNhanVien: 'nv-0011',
        hoVaTen: 'Đặng Văn Dũng',
        gioiTinh: 'Nữ',
        ngaySinh: '28/02/1982',
        email: 'đặng11@example.com',
        diaChi: '84 Đội Cấn'
    },
    {
        maNhanVien: 'nv-0012',
        hoVaTen: 'Hoàng Thị Mai',
        gioiTinh: 'Nữ',
        ngaySinh: '17/05/1989',
        email: 'hoàng12@example.com',
        diaChi: '157 Đội Cấn'
    },
    {
        maNhanVien: 'nv-0013',
        hoVaTen: 'Trần Văn Hùng',
        gioiTinh: 'Nam',
        ngaySinh: '20/03/1990',
        email: 'trần13@example.com',
        diaChi: '31 Láng Hạ'
    },
    {
        maNhanVien: 'nv-0014',
        hoVaTen: 'Lê Văn An',
        gioiTinh: 'Nam',
        ngaySinh: '14/04/1973',
        email: 'lê14@example.com',
        diaChi: '187 Đội Cấn'
    },
    {
        maNhanVien: 'nv-0015',
        hoVaTen: 'Nguyễn Thị Hương',
        gioiTinh: 'Nam',
        ngaySinh: '08/04/1995',
        email: 'nguyễn15@example.com',
        diaChi: '18 Cầu Giấy'
    },
    {
        maNhanVien: 'nv-0016',
        hoVaTen: 'Vũ Thị Ngọc',
        gioiTinh: 'Nam',
        ngaySinh: '08/03/1972',
        email: 'vũ16@example.com',
        diaChi: '177 Kim Mã'
    },
    {
        maNhanVien: 'nv-0017',
        hoVaTen: 'Nguyễn Văn Dũng',
        gioiTinh: 'Nữ',
        ngaySinh: '12/09/2000',
        email: 'nguyễn17@example.com',
        diaChi: '123 Kim Mã'
    },
    {
        maNhanVien: 'nv-0018',
        hoVaTen: 'Bùi Thị Mai',
        gioiTinh: 'Nam',
        ngaySinh: '14/09/1973',
        email: 'bùi18@example.com',
        diaChi: '76 Trần Duy Hưng'
    },
    {
        maNhanVien: 'nv-0019',
        hoVaTen: 'Lê Văn Hùng',
        gioiTinh: 'Nam',
        ngaySinh: '05/08/1991',
        email: 'lê19@example.com',
        diaChi: '41 Kim Mã'
    },
    {
        maNhanVien: 'nv-0020',
        hoVaTen: 'Trần Thị Lan',
        gioiTinh: 'Nữ',
        ngaySinh: '02/10/1986',
        email: 'trần20@example.com',
        diaChi: '98 Tôn Thất Thuyết'
    },
    {
        maNhanVien: 'nv-0021',
        hoVaTen: 'Phạm Thị Hoa',
        gioiTinh: 'Nữ',
        ngaySinh: '14/10/1976',
        email: 'phạm21@example.com',
        diaChi: '179 Nguyễn Chí Thanh'
    },
    {
        maNhanVien: 'nv-0022',
        hoVaTen: 'Đỗ Thị Hương',
        gioiTinh: 'Nam',
        ngaySinh: '10/12/1992',
        email: 'đỗ22@example.com',
        diaChi: '137 Đội Cấn'
    },
    {
        maNhanVien: 'nv-0023',
        hoVaTen: 'Trần Thị Mai',
        gioiTinh: 'Nam',
        ngaySinh: '13/04/1999',
        email: 'trần23@example.com',
        diaChi: '61 Phùng Khoang'
    },
    {
        maNhanVien: 'nv-0024',
        hoVaTen: 'Nguyễn Văn Hùng',
        gioiTinh: 'Nam',
        ngaySinh: '13/05/1994',
        email: 'nguyễn24@example.com',
        diaChi: '53 Láng Hạ'
    },
    {
        maNhanVien: 'nv-0025',
        hoVaTen: 'Đỗ Văn Hùng',
        gioiTinh: 'Nam',
        ngaySinh: '30/12/1982',
        email: 'đỗ25@example.com',
        diaChi: '145 Trần Duy Hưng'
    },
    {
        maNhanVien: 'nv-0026',
        hoVaTen: 'Phạm Văn Bình',
        gioiTinh: 'Nam',
        ngaySinh: '25/08/1996',
        email: 'phạm26@example.com',
        diaChi: '25 Phùng Khoang'
    },
    {
        maNhanVien: 'nv-0027',
        hoVaTen: 'Trần Văn Sơn',
        gioiTinh: 'Nam',
        ngaySinh: '29/11/1999',
        email: 'trần27@example.com',
        diaChi: '136 Phùng Khoang'
    },
    {
        maNhanVien: 'nv-0028',
        hoVaTen: 'Vũ Văn Sơn',
        gioiTinh: 'Nữ',
        ngaySinh: '29/01/1974',
        email: 'vũ28@example.com',
        diaChi: '19 Xuân Thủy'
    },
    {
        maNhanVien: 'nv-0029',
        hoVaTen: 'Phạm Thị Hương',
        gioiTinh: 'Nữ',
        ngaySinh: '03/05/1982',
        email: 'phạm29@example.com',
        diaChi: '149 Cầu Giấy'
    },
    {
        maNhanVien: 'nv-0030',
        hoVaTen: 'Hoàng Văn Sơn',
        gioiTinh: 'Nữ',
        ngaySinh: '18/09/1974',
        email: 'hoàng30@example.com',
        diaChi: '150 Xuân Thủy'
    },
    {
        maNhanVien: 'nv-0031',
        hoVaTen: 'Nguyễn Văn Hùng',
        gioiTinh: 'Nam',
        ngaySinh: '21/10/1989',
        email: 'nguyễn31@example.com',
        diaChi: '129 Láng Hạ'
    },
    {
        maNhanVien: 'nv-0032',
        hoVaTen: 'Đỗ Văn Hùng',
        gioiTinh: 'Nữ',
        ngaySinh: '31/03/1972',
        email: 'đỗ32@example.com',
        diaChi: '170 Xuân Thủy'
    },
    {
        maNhanVien: 'nv-0033',
        hoVaTen: 'Lê Văn Bình',
        gioiTinh: 'Nữ',
        ngaySinh: '12/09/1986',
        email: 'lê33@example.com',
        diaChi: '30 Trần Duy Hưng'
    },
    {
        maNhanVien: 'nv-0034',
        hoVaTen: 'Trần Thị Mai',
        gioiTinh: 'Nam',
        ngaySinh: '01/04/1989',
        email: 'trần34@example.com',
        diaChi: '175 Cầu Giấy'
    },
    {
        maNhanVien: 'nv-0035',
        hoVaTen: 'Phan Văn Hùng',
        gioiTinh: 'Nữ',
        ngaySinh: '21/02/1975',
        email: 'phan35@example.com',
        diaChi: '182 Tôn Thất Thuyết'
    },
    {
        maNhanVien: 'nv-0036',
        hoVaTen: 'Đặng Thị Hương',
        gioiTinh: 'Nữ',
        ngaySinh: '01/02/1981',
        email: 'đặng36@example.com',
        diaChi: '139 Tôn Thất Thuyết'
    },
    {
        maNhanVien: 'nv-0037',
        hoVaTen: 'Nguyễn Thị Mai',
        gioiTinh: 'Nam',
        ngaySinh: '25/03/1970',
        email: 'nguyễn37@example.com',
        diaChi: '33 Xuân Thủy'
    },
    {
        maNhanVien: 'nv-0038',
        hoVaTen: 'Hoàng Văn Bình',
        gioiTinh: 'Nam',
        ngaySinh: '03/04/1995',
        email: 'hoàng38@example.com',
        diaChi: '150 Láng Hạ'
    },
    {
        maNhanVien: 'nv-0039',
        hoVaTen: 'Hoàng Thị Mai',
        gioiTinh: 'Nam',
        ngaySinh: '02/05/1980',
        email: 'hoàng39@example.com',
        diaChi: '159 Thái Hà'
    },
    {
        maNhanVien: 'nv-0040',
        hoVaTen: 'Bùi Văn An',
        gioiTinh: 'Nam',
        ngaySinh: '28/01/1990',
        email: 'bùi40@example.com',
        diaChi: '87 Phùng Khoang'
    },
    {
        maNhanVien: 'nv-0041',
        hoVaTen: 'Trần Thị Mai',
        gioiTinh: 'Nam',
        ngaySinh: '23/11/1988',
        email: 'trần41@example.com',
        diaChi: '25 Láng Hạ'
    },
    {
        maNhanVien: 'nv-0042',
        hoVaTen: 'Phan Văn An',
        gioiTinh: 'Nữ',
        ngaySinh: '06/10/1995',
        email: 'phan42@example.com',
        diaChi: '162 Kim Mã'
    },
    {
        maNhanVien: 'nv-0043',
        hoVaTen: 'Bùi Văn An',
        gioiTinh: 'Nam',
        ngaySinh: '28/04/1997',
        email: 'bùi43@example.com',
        diaChi: '12 Thái Hà'
    },
    {
        maNhanVien: 'nv-0044',
        hoVaTen: 'Lê Văn Sơn',
        gioiTinh: 'Nữ',
        ngaySinh: '03/11/1982',
        email: 'lê44@example.com',
        diaChi: '54 Nguyễn Chí Thanh'
    },
    {
        maNhanVien: 'nv-0045',
        hoVaTen: 'Hoàng Văn An',
        gioiTinh: 'Nam',
        ngaySinh: '29/05/1986',
        email: 'hoàng45@example.com',
        diaChi: '144 Thái Hà'
    },
    {
        maNhanVien: 'nv-0046',
        hoVaTen: 'Hoàng Thị Hương',
        gioiTinh: 'Nữ',
        ngaySinh: '12/11/1974',
        email: 'hoàng46@example.com',
        diaChi: '10 Phùng Khoang'
    },
    {
        maNhanVien: 'nv-0047',
        hoVaTen: 'Vũ Thị Ngọc',
        gioiTinh: 'Nam',
        ngaySinh: '06/05/2001',
        email: 'vũ47@example.com',
        diaChi: '52 Đội Cấn'
    },
    {
        maNhanVien: 'nv-0048',
        hoVaTen: 'Phạm Văn Sơn',
        gioiTinh: 'Nữ',
        ngaySinh: '13/08/1986',
        email: 'phạm48@example.com',
        diaChi: '21 Thái Hà'
    },
    {
        maNhanVien: 'nv-0049',
        hoVaTen: 'Phạm Thị Ngọc',
        gioiTinh: 'Nam',
        ngaySinh: '09/04/1992',
        email: 'phạm49@example.com',
        diaChi: '59 Láng Hạ'
    },
    {
        maNhanVien: 'nv-0050',
        hoVaTen: 'Bùi Văn Hùng',
        gioiTinh: 'Nam',
        ngaySinh: '14/10/1971',
        email: 'bùi50@example.com',
        diaChi: '29 Thái Hà'
    }
]

function insertTable(employees) {
    const tbody = document.querySelector('.employee-table tbody')

    employees.forEach((employee, index) => {
        const row = document.createElement('tr')
        row.innerHTML = `
      <td>${index + 1}</td>
      <td>${employee.maNhanVien}</td>
      <td>${employee.hoVaTen}</td>
      <td>${employee.gioiTinh}</td>
      <td>${employee.ngaySinh}</td>
      <td>${employee.email}</td>
      <td>
              <div class = "action">
                <span> ${employee.diaChi} </span>
                <div class="action-buttons">
                  <button class="action-button">
                    <img src="../../assets/icon/success-48.png" alt="logo" />
                  </button>
                  <button class="action-button">
                    <img src="../../assets/icon/info-48.png" alt="logo" />
                  </button>
                  <button class="action-button">
                    <img src="../../assets/icon/delete-48.png" alt="logo" />
                  </button>
                </div>
              </div>
            </td>
    `;
        tbody.appendChild(row)
    })
}


function insertTotalEmployee(totalEmployees) {
    const total = document.querySelector('.footer-table .footer-section')
    const value = document.createElement('span')
    value.innerHTML = `Tổng: ${totalEmployees}`
    total.appendChild(value)
}

function toggleSidebar() {
    const sidebar = document.getElementById('sidebar')
    const toggleButton = document.getElementById('toggleButton')
    const buttonImg = toggleButton.querySelector('img')

    toggleButton.addEventListener('click', function () {
        sidebar.classList.toggle('sidebar--less');
        if (sidebar.classList.contains('sidebar--less')) {
            buttonImg.src = '../../assets/icon/btn-next-page.svg'
        } else {
            buttonImg.src = '../../assets/icon/btn-prev-page.svg'
        }
        console.log('toggle')
    });
}

document.addEventListener('DOMContentLoaded', () => {
    insertTable(employees)
    insertTotalEmployee(employees.length)
    toggleSidebar()
});