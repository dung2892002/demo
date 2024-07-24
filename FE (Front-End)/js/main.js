const baseUrl = "https://localhost:7204/api/v1";

var pageNumber = 1;
var totalPage = 0;
var searchValue = '';
var pageSize = 10;
var employees = [];

async function fetchEmployees() {
    try {
        const params = new URLSearchParams({
            pageSize: pageSize,
            pageNumber: pageNumber,
            employeeFilter: searchValue
        });
        const response = await fetch(`${baseUrl}/Employees/filter?${params.toString()}`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        employees = data.Data
        totalPage = data.TotalPages;
        renderEmployeeTable(employees);
        displayPageInfo(data.TotalRecords);
    } catch (error) {
        console.error('Error:', error);
    }
}

async function deleteEmployee(employeeId) {
    try {
        const response = await fetch(`${baseUrl}/Employees/${employeeId}`, {
            method: 'DELETE'
        });
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        fetchEmployees();
    } catch (error) {
        console.error('Error:', error);
    }
}


function setupDeleteButton() {
    document.querySelectorAll('.button--delete').forEach(button => {
        button.addEventListener('click', (event) => {
            const confirmation = confirm(`bạn có chắc muốn xóa nhân viên này không`);
            if (confirmation) {
                console.log('delete employee');
                const employeeId = event.currentTarget.getAttribute('data-id');
                deleteEmployee(employeeId);
                alert('Xóa thành công nhân viên');
            }
        });
    });
}

function setupEditButton() {
    document.querySelectorAll('.button--edit').forEach(button => {
        button.addEventListener('click', () => {
            const employeeId = button.getAttribute('data-id');
            const employee = employees.find(emp => emp.EmployeeId === employeeId);
            showEmployeeForm(true, employee);
        });
    });
}

function renderEmployeeTable(employees) {
    const tbody = document.querySelector('.employee-table tbody');
    tbody.innerHTML = '';
    employees.forEach((employee, index) => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${index + 1}</td>
            <td>${employee.EmployeeCode}</td>
            <td>${employee.Fullname}</td>
            <td>${employee.GenderName}</td>
            <td>${formatDate(employee.DateOfBirth)}</td>
            <td>${employee.Email}</td>
            <td>
                <div class="action">
                    <span>${employee.Address}</span>
                    <div class="action-buttons">
                        <button class="action-button button--edit" data-id="${employee.EmployeeId}">
                            <img src="../../assets/icon/icons8-pencil-58.png" alt="logo" class="icon"/>
                        </button>
                        <button class="action-button">
                            <img src="../../assets/icon/icons8-duplicate-50.png" alt="logo" class="icon"/>
                        </button>
                        <button class="action-button button--delete" id="deleteButton" data-id="${employee.EmployeeId}">
                            <img src="../../assets/icon/delete-48.png" alt="logo" class="icon"/>
                        </button>
                    </div>
                </div>
            </td>
        `;
        tbody.appendChild(row);
    });
    setupDeleteButton();
    setupEditButton();
}

function displayPageInfo(totalEmployees) {
    const total = document.querySelector('.pagination .pagination-section');
    total.innerHTML = '';
    const value = document.createElement('span');
    document.querySelector('.current-page').innerHTML = pageNumber;
    value.innerHTML = `Tổng: ${totalEmployees}`;
    total.appendChild(value);
}

function handleNextPage() {
    if (pageNumber < totalPage) pageNumber++;
    fetchEmployees();
}

function handlePrevPage() {
    if (pageNumber > 1) pageNumber--;
    fetchEmployees();
}

function handleSearch() {
    pageNumber = 1;
    searchValue = document.getElementById('searchEmployee').value;
    fetchEmployees();
}

function updatePageSize() {
    pageNumber = 1;
    pageSize = document.getElementById('records-per-page').value;
    fetchEmployees();
}

function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const toggleButton = document.getElementById('toggleButton');
    const buttonImg = toggleButton.querySelector('img');

    toggleButton.addEventListener('click', function () {
        sidebar.classList.toggle('sidebar--less');
        if (sidebar.classList.contains('sidebar--less')) {
            buttonImg.src = '../../assets/icon/btn-next-page.svg';
        } else {
            buttonImg.src = '../../assets/icon/btn-prev-page.svg';
        }
    });
}

document.addEventListener('DOMContentLoaded', () => {
    toggleSidebar();
    fetchEmployees();
    fetchDepartments();
    fetchPositions();
    document.getElementById('records-per-page').addEventListener('change', updatePageSize);
    document.getElementById('searchEmployee').addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            event.preventDefault();
            handleSearch();
        }
    });
});
