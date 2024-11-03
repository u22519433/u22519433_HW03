$(document).ready(function () {
    var editDescriptionModal = new bootstrap.Modal(document.getElementById('editDescriptionModal'));
    var currentReportData = null;
    var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

    // Handle report form submission
    $('#reportForm').on('submit', function (e) {
        e.preventDefault();

        var formData = {
            reportType: $('select[name="reportType"]').val(),
            startDate: $('input[name="startDate"]').val(),
            endDate: $('input[name="endDate"]').val(),
            __RequestVerificationToken: $('Input[name="__RequestVerificationToken"]').val()
        };

        // Show loading indicator
        $('#reportContent').html('<div class="text-center"><div class="spinner-border" role="status"></div><div>Generating Report...</div></div>');

        $.ajax({
            url: '/Report/GenerateReport',
            method: 'POST',
            data: formData,
            success: function (response) {
                if (response.success && response.items) {
                    currentReportData = response.items;
                    displayReport(response.items);
                    $('#saveReportSection').show();
                } else {
                    $('#reportContent').html('<div class="alert alert-warning">No data found for the selected criteria.</div>');
                    $('#saveReportSection').hide();
                }
            },
            error: function (xhr, status, error) {
                console.log('Error:', error);
                console.log('Response:', xhr.responseText);
                $('#reportContent').html('<div class="alert alert-danger">Error generating report. Please try again.</div>');
                $('#saveReportSection').hide();
            }
        });
    });

    function displayReport(data) {
        var reportType = $('select[name="reportType"]').val();
        var content = '<div class="card">';

        // Add report header
        content += `<div class="card-header">
                    <h4>${getReportTitle(reportType)}</h4>
                    <p class="text-muted mb-0">Generated on ${new Date().toLocaleDateString()}</p>
                </div>
                <div class="card-body">`;

        // Add table with appropriate styling
        content += '<div class="table-responsive"><table class="table table-striped table-bordered">';

        switch (reportType) {
            case 'BorrowingHistory':
                content += `
                <thead class="table-dark">
                    <tr>
                        <th>Student</th>
                        <th>Book</th>
                        <th>Borrow Date</th>
                        <th>Return Date</th>
                        <th>Days Kept</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>`;

                data.forEach(function (item) {
                    content += `
                    <tr>
                        <td>${item.studentName}</td>
                        <td>${item.bookName}</td>
                        <td>${new Date(item.borrowDate).toLocaleDateString()}</td>
                        <td>${item.returnDate ? new Date(item.returnDate).toLocaleDateString() : 'Not Returned'}</td>
                        <td>${item.daysKept}</td>
                        <td>
                            <span class="badge ${item.isOverdue ? 'bg-danger' : 'bg-success'}">
                                ${item.isOverdue ? 'Overdue' : 'On Time'}
                            </span>
                        </td>
                    </tr>`;
                });
                break;

            case 'OverdueBooks':
                content += `
                <thead class="table-dark">
                    <tr>
                        <th>Student</th>
                        <th>Book</th>
                        <th>Borrow Date</th>
                        <th>Days Overdue</th>
                    </tr>
                </thead>
                <tbody>`;

                data.forEach(function (item) {
                    content += `
                    <tr>
                        <td>${item.studentName}</td>
                        <td>${item.bookName}</td>
                        <td>${new Date(item.borrowDate).toLocaleDateString()}</td>
                        <td>
                            <span class="badge bg-danger">${item.daysOverdue} days</span>
                        </td>
                    </tr>`;
                });
                break;

            case 'PopularBooks':
                // Add chart
                content += '<div class="mb-4"><canvas id="popularBooksChart"></canvas></div>';

                content += `
                <thead class="table-dark">
                    <tr>
                        <th>Book</th>
                        <th>Times Borrowed</th>
                        <th>Status</th>
                        <th>Popularity</th>
                    </tr>
                </thead>
                <tbody>`;

                data.forEach(function (item) {
                    content += `
                    <tr>
                        <td>${item.bookName}</td>
                        <td>${item.borrowCount}</td>
                        <td>
                            <span class="badge ${item.isAvailable ? 'bg-success' : 'bg-warning'}">
                                ${item.isAvailable ? 'Available' : 'Borrowed'}
                            </span>
                        </td>
                        <td>
                            <div class="progress">
                                <div class="progress-bar" role="progressbar" 
                                     style="width: ${item.popularityScore}%"
                                     aria-valuenow="${item.popularityScore}" 
                                     aria-valuemin="0" 
                                     aria-valuemax="100">
                                    ${item.popularityScore}%
                                </div>
                            </div>
                        </td>
                    </tr>`;
                });
                break;

            case 'UnreturnedBooks':
                content += `
                <thead class="table-dark">
                    <tr>
                        <th>Book</th>
                        <th>Student</th>
                        <th>Borrowed Since</th>
                        <th>Days Outstanding</th>
                    </tr>
                </thead>
                <tbody>`;

                data.forEach(function (item) {
                    content += `
                    <tr>
                        <td>${item.bookName}</td>
                        <td>${item.studentName}</td>
                        <td>${new Date(item.borrowDate).toLocaleDateString()}</td>
                        <td>
                            <span class="badge ${item.daysOutstanding > 14 ? 'bg-danger' : 'bg-warning'}">
                                ${item.daysOutstanding} days
                            </span>
                        </td>
                    </tr>`;
                });
                break;
        }

        content += '</tbody></table></div>';

        // Add summary section if needed
        if (data.length > 0) {
            content += `
            <div class="card-footer bg-light mt-3">
                <div class="row">
                    <div class="col">
                        <strong>Total Records:</strong> ${data.length}
                    </div>
                </div>
            </div>`;
        } else {
            content += '<div class="alert alert-info">No records found for the selected criteria.</div>';
        }

        content += '</div></div>';

        $('#reportContent').html(content);

       
        if (reportType === 'PopularBooks' && data.length > 0) {
            var ctx = document.getElementById('popularBooksChart').getContext('2d');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: data.map(item => item.bookName),
                    datasets: [{
                        label: 'Number of Times Borrowed',
                        data: data.map(item => item.borrowCount),
                        backgroundColor: 'rgba(54, 162, 235, 0.5)',
                        borderColor: 'rgba(54, 162, 235, 1)',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: {
                                stepSize: 1
                            }
                        }
                    }
                }
            });
        }
    }

    // Helper function for report titles
    function getReportTitle(reportType) {
        switch (reportType) {
            case 'BorrowingHistory':
                return 'Library Borrowing History Report';
            case 'OverdueBooks':
                return 'Overdue Books Analysis Report';
            case 'PopularBooks':
                return 'Book Popularity Analysis Report';
            case 'UnreturnedBooks':
                return 'Currently Unreturned Books Report';
            default:
                return 'Library Report';
        }
    }



    // Handle save report form submission
    $('#saveReportForm').on('submit', function (e) {
        e.preventDefault();

        if (!currentReportData) {
            alert('Please generate a report first.');
            return;
        }

        var formData = {
            reportData: JSON.stringify(currentReportData),
            fileName: $('input[name="fileName"]').val(),
            fileType: $('select[name="fileType"]').val(),
            __RequestVerificationToken: antiForgeryToken
        };

        $.ajax({
            url: '/Report/SaveReport',
            method: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    location.reload();
                } else {
                    alert('Error saving report: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                console.log('Error:', error);
                alert('Error saving report. Please try again.');
            }
        });
    });

    // Helper functions
    function formatDate(dateString) {
        return new Date(dateString).toLocaleDateString();
    }

    function getReportTitle(reportType) {
        switch (reportType) {
            case 'BorrowingHistory': return 'Borrowing History Report';
            case 'OverdueBooks': return 'Overdue Books Report';
            case 'PopularBooks': return 'Popular Books Report';
            case 'UnreturnedBooks': return 'Unreturned Books Report';
            default: return 'Library Report';
        }
    }

    function initializeChart(data) {
        var ctx = document.getElementById('reportChart').getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.map(item => item.bookName),
                datasets: [{
                    label: 'Times Borrowed',
                    data: data.map(item => item.borrowCount),
                    backgroundColor: 'rgba(54, 162, 235, 0.5)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                }
            }
        });
    }

    // Document functions
    window.downloadReport = function (reportId) {
        window.location.href = '/Report/DownloadReport/' + reportId;
    };

    window.deleteReport = function (reportId) {
        if (confirm('Are you sure you want to delete this report?')) {
            $.ajax({
                url: '/Report/DeleteReport',
                method: 'POST',
                data: {
                    id: reportId,
                    __RequestVerificationToken: antiForgeryToken
                },
                success: function (response) {
                    if (response.success) {
                        location.reload();
                    } else {
                        alert('Error deleting report.');
                    }
                },
                error: function () {
                    alert('Error deleting report.');
                }
            });
        }
    };

    window.editDescription = function (reportId) {
        $('#editReportId').val(reportId);
        var currentDescription = $(`[data-report-id="${reportId}"]`).find('.description').text();
        $('#reportDescription').val(currentDescription);
        editDescriptionModal.show();
    };

    window.saveDescription = function () {
        var formData = {
            reportId: $('#editReportId').val(),
            description: $('#reportDescription').val(),
            __RequestVerificationToken: antiForgeryToken
        };

        $.ajax({
            url: '/Report/UpdateDescription',
            method: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    location.reload();
                } else {
                    alert('Error updating description.');
                }
            },
            error: function () {
                alert('Error updating description.');
            }
        });
    };
});

