var newEmployeeCode = null;
var departmentsList = [];
var positionList = [];
var isEditMode = false;
var currentEmployeeId = null;


async function showEmployeeForm(isEdit, employee) {
    const employeeForm = document.getElementById('employeeForm');
    employeeForm.style.display = 'block';

    isEditMode = isEdit;
    currentEmployeeId = employee ? employee.EmployeeId : null;

    if (isEditMode && employee) {
        fillEmployeeForm(employee);
    } else {
        fetchNewEmployeeCode();
    }

};

function hideEmployeeForm() {
    document.getElementById('employeeForm').style.display = 'none';
    resetEmployeeForm();
};

window.onclick = function (event) {
    if (event.target === document.getElementById('employeeForm')) {
        hideEmployeeForm();
    }
};

async function fetchNewEmployeeCode() {
    try {
        const response = await fetch(`${baseUrl}/Employees/NewEmployeeCode`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const employeeCodeText = await response.text();
        const employeeCode = document.getElementById('employee-code')
        employeeCode.value = employeeCodeText;
    } catch (error) {
        console.error('Error:', error);
    }
}

async function fetchDepartments() {
    try {
        const response = await fetch(`${baseUrl}/Departments`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const departments = await response.json();
        departmentsList = departments

        const departmentSelect = document.getElementById('department');

        departments.forEach(department => {
            const option = document.createElement('option');
            option.value = department.DepartmentId;
            option.textContent = department.DepartmentName;
            departmentSelect.appendChild(option);
        });

    } catch (error) {
        console.error('Error:', error);
    }
}

async function fetchPositions() {
    try {
        const response = await fetch(`${baseUrl}/Positions`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const positions = await response.json();
        positionList = positions

        const positionSelect = document.getElementById('position');

        positions.forEach(position => {
            const option = document.createElement('option');
            option.value = position.PositionId;
            option.textContent = position.PositionName;
            positionSelect.appendChild(option);
        });

    } catch (error) {
        console.error('Error:', error);
    }
}


function getSelectedGender() {
    const genderRadios = document.getElementsByName('gender');
    for (const radio of genderRadios) {
        if (radio.checked) {
            return radio.value;
        }
    }
    return null;
}

function resetEmployeeForm() {
    document.getElementById('form').reset();
}

function fillEmployeeForm(employee) {
    document.getElementById('employee-code').value = employee.EmployeeCode;
    document.getElementById('fullname').value = employee.Fullname;
    document.getElementById('date-of-birth').value = formatDateForm(employee.DateOfBirth);
    document.querySelector(`input[name="gender"][value="${employee.Gender}"]`).checked = true;

    document.getElementById('identity-number').value = employee.IdentityNumber;
    document.getElementById('identity-date').value = formatDateForm(employee.IdentityDate);
    document.getElementById('identity-place').value = employee.IdentityPlace;

    document.getElementById('mobile-number').value = employee.MobileNumber;
    document.getElementById('landline-number').value = employee.LandlineNumber;
    document.getElementById('email').value = employee.Email;

    document.getElementById('address').value = employee.Address;

    document.getElementById('bank-number').value = employee.BankNumber;
    document.getElementById('bank-name').value = employee.BankName;
    document.getElementById('bank-branch').value = employee.BankBranch;

    document.getElementById('department').value = employee.DepartmentId;
    document.getElementById('position').value = employee.PositionId;

}

async function submitForm(event) {
    event.preventDefault();

    const selectedDepartmentId = document.getElementById('department').value;
    const selectedPositionId = document.getElementById('position').value;
    const selectedGender = getSelectedGender();

    const requestBody = {
        EmployeeCode: document.getElementById('employee-code').value,
        Fullname: document.getElementById('fullname').value,
        DateOfBirth: document.getElementById('date-of-birth').value,
        Gender: selectedGender,
        IdentityNumber: document.getElementById('identity-number').value,
        IdentityDate: document.getElementById('identity-date').value,
        IdentityPlace: document.getElementById('identity-place').value,
        MobileNumber: document.getElementById('mobile-number').value,
        LandlineNumber: document.getElementById('landline-number').value,
        Email: document.getElementById('email').value,
        Address: document.getElementById('address').value,
        BankNumber: document.getElementById('bank-number').value,
        BankName: document.getElementById('bank-name').value,
        BankBranch: document.getElementById('bank-branch').value,
        DepartmentId: selectedDepartmentId,
        PositionId: selectedPositionId,
        CreatedBy: "admin",
        ModifiedBy: "admin",
    };

    if (!isEditMode) {
        try {
            const response = await fetch(`${baseUrl}/Employees`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            });

            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            alert('Thêm mới nhân viên thành công');
            hideEmployeeForm();
            fetchEmployees();
        } catch (error) {
            console.error('Error:', error);
        }
    } else {
        try {
            const response = await fetch(`${baseUrl}/Employees/${currentEmployeeId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            });
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            alert('Chỉnh sửa thông tin nhân viên thành công');
            hideEmployeeForm();
            fetchEmployees();
        } catch (error) {
            alert('có lỗi xảy ra');
            console.error('Error:', error);
        }
    }
}

function handleSubmit() {
    const submitButton = document.getElementById('submitButton');
    submitButton.addEventListener('click', submitForm);
}

document.addEventListener('DOMContentLoaded', () => {
    handleSubmit();
});