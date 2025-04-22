using System;
using DB.Application.Features.Employees.Queries.ViewModels;
using DB.Domain.Entities;

namespace DB.Application.Contracts.Factories
{
    public interface IEmployeeViewModelFactory
    {
        EmployeeViewModel CreateViewModel(Employee employee);
    }
}
