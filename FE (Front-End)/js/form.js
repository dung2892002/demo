var newEmployeeCode = null;
var departmentsList = [];
var positionList = [];
var isEditMode = false;
var currentEmployeeId = null;


function showEmployeeForm(isEdit, employee) {
    const employeeForm = document.getElementById('employeeForm');
    employeeForm.style.display = 'block';
    isEditMode = isEdit;
    currentEmployeeId = employee ? employee.EmployeeId : null;

    if (isEditMode && employee) {
        fillEmployeeForm(employee);
    } else {
        fetchNewEmployeeCode();
    }
    fetchDepartments();
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
        departmentSelect.innerHTML = '<option value=""></option>';

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
    document.getElementById('fullname').value = employee.FullName;
    document.getElementById('date-of-birth').value = formatDateForm(employee.DateOfBirth);
    document.getElementById('mobile-number').value = employee.PhoneNumber;
    document.getElementById('email').value = employee.Email;
    document.getElementById('address').value = employee.Address;
    document.getElementById('identity-number').value = employee.IdentityNumber;
    document.getElementById('identity-date').value = formatDateForm(employee.IdentityDate);
    document.getElementById('identity-place').value = employee.IdentityPlace;
    document.getElementById('department').value = employee.DepartmentName;

    document.querySelector(`input[name="gender"][value="${employee.Gender}"]`).checked = true;
}

async function submitForm(event) {
    event.preventDefault();

    const selectedDepartmentId = document.getElementById('department').value;
    const selectedDepartment = departmentsList.find(dept => dept.DepartmentId === selectedDepartmentId);
    const selectedGender = getSelectedGender();

    const requestBody = {
        createdDate: new Date().toISOString(),
        modifiedDate: new Date().toISOString(),
        employeeCode: document.getElementById('employee-code').value,
        fullName: document.getElementById('fullname').value,
        gender: selectedGender,
        dateOfBirth: document.getElementById('date-of-birth').value,
        phoneNumber: document.getElementById('mobile-number').value,
        email: document.getElementById('email').value,
        address: document.getElementById('address').value,
        identityNumber: document.getElementById('identity-number').value,
        identityDate: document.getElementById('identity-date').value,
        identityPlace: document.getElementById('identity-place').value,
        joinDate: new Date().toISOString(),
        departmentId: selectedDepartmentId,
        positionId: null,
        departmentCode: selectedDepartment.DepartmentCode,
        departmentName: selectedDepartment.DepartmentName,
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