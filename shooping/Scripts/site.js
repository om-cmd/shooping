// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
async function fetchData(url, isList = false) {
    var totalItems = 0;
    const response = await fetch(url,
        {
            "headers":
            {
                "X-Auth-Token": localStorage.auth_token||""
            }

        }
    ).then(res => {
        totalItems = res.headers.get("X-TotalItems")
        return res.json()
    }, err => alert("error"));
    if (isList)
        return [response, totalItems]
    return (response);
}

async function postData(url, body, onSuccess = res => res.json(), onError = (e) => { alert(e.responseText) }) {
    $.ajax({
        type: 'POST',
        url: url,
        data: typeof body == 'string' ? body : JSON.stringify(body), // or JSON.stringify ({name: 'jonas'}),
        success: onSuccess,
        error: onError,
        contentType: "application/json",
        dataType: 'json',
        headers: { "X-Auth-Token": localStorage.auth_token || "" }
    });
}
async function DeleteData(url, id, onSuccess = res => res.json(), onError = (e) => { alert(e.responseText) }) {
    if (confirm("CONFIRM DELETE?")) {
        $.ajax({
            type: 'DELETE',
            url: `${url}/${id}`,
            success: onSuccess,
            error: onError,
            contentType: "application/json",
            dataType: 'json',
            headers: { "X-Auth-Token": localStorage.auth_token || "" }
        });
    }
}

async function PutData(url, body, onSuccess = res => res.json(), onError = (e) => { alert(e.responseText) }) {
    $.ajax({
        type: 'PUT',
        url: url,
        success: onSuccess,
        data: typeof body == 'string' ? body : JSON.stringify(body),
        error: onError,
        contentType: "application/json",
        dataType: 'json',
        headers: { "X-Auth-Token": localStorage.auth_token || "" }
    });
}
var Site = Site || {};

Site.populateDropdown = function (elementID, data) {
    const element = $("#" + elementID);
    var options = "";
    if (elementID == "customer") {

        data.forEach(item => {
            options += `<option value="${item.CustomerID}">${item.Name}</option>`;

        });
    } else {
        data.forEach(item => {
            options += `<option value="${item.ProductID}">${item.Name}</option>`;

        });
    }

    element.html(options);
};
function populateList(data, moduleName) {
    const tbody = $('#tableBody');
    let htmlstr = "";

    data = typeof data == 'string' ? JSON.parse(data) : data;
    console.log(data);
    data.forEach(item => {
        var row;

        switch (moduleName) {
            case 'Customers':
                row = `<tr>
                            <td>${item.Name}</td>
                            <td>${item.Email}</td>
                            <td>${item.Gender}</td>
                            <td>${item.IsActive}</td>
                            <td>${item.DateOfBirth ? parseDate(item.DateOfBirth) : ''}</td>
                            <td>${getStarsHTML(item.Ratings) }</td>
                            
                            <td>
                                <button class="btn btn-info" onclick="openModal('${item.CustomerID}')">Edit</button>
                            
                            
                                <button  class="btn btn-danger"onclick="deleteCustomer('${item.CustomerID}')">Delete</button>
                            </td>
                        </tr>`;
                break;
            case 'Products':
                row = `<tr>
                            <td>${item.Name}</td>
                            <td>${item.Price}</td>
                            <td>${getStarsHTML(item.Ratings) }</td>
                            <td>${item.ExpDate ? parseDate(item.ExpDate) : ''}</td>
                            <td>${item.IsGrocery}</td>
                            <td>
                                <button class="btn btn-info" onclick="openModal('${item.ProductID}')">Edit</button>
                            
                            
                                <button class="btn btn-danger" onclick="deleteProduct('${item.ProductID}')">Delete</button>
                            </td>
                        </tr>`;
                break;
            case 'Sales':
                row = `<tr>
                            <td>${item.Customer.Name}</td>
                            <td>${item.Product.Name}</td>
                            <td>${item.Quantity}</td>
                            <td>${item.TotalPrice}</td>
                            <td>${item.InCash}</td>
                            <td>${getStarsHTML(item.Ratings) }</td>
                            <td>${item.SaleDate ? parseDate(item.SaleDate) : '' }</td>
                            <td>
                                <button class="btn btn-info" onclick="openModal('${item.SaleID}')">Edit</button>
                            
                            
                                <button class="btn btn-danger" onclick="deleteSale('${item.SaleID}')">Delete</button>
                            </td>
                        </tr>`;
                break;
        }

        htmlstr += row;
    });

    tbody.html(htmlstr);
}

function parseDate(dateString) {
    //"/Date(1704651300000)/"
   
    if (dateString != null)
    {
        const str = dateString.substr(6, 13);
        console.log(str);
    const date = moment(parseInt(str));
    return date.format("yyyy-MM-DD")
    }
}

function getStarsHTML(number) {

    var HTMLstar = "";
    for (var i = 1; i <= 5; i++)
    {
        if (i <= number) {
            HTMLstar += `<i class="bi bi-star-fill"></i>`
        }
        else if (i > number && (i-1) < number)
        {
            HTMLstar += `<i class="bi bi-star-half"></i>`
        }
        else
        {
            HTMLstar += `<i class="bi bi-star"></i>`
        }

    }
    return HTMLstar;

}     

//function logout() {
//    if (confirm("Do you want to logout?")) {
//        localStorage.auth_token = "";
//        localStorage.user = "{}";
//        window.location.href = "https://localhost:44301/login";
//    }
//}

function logout() {
    if (confirm("Do you want to logout?")) {
        $.ajax({
            type: "POST",
            url: "/Login/Logout", 
            success: function (response) {
                localStorage.auth_token = "";
                localStorage.user = "{}";
                window.location.href = "https://localhost:44301/login";
            },
            error: function (xhr, status, error) {
                console.error("Logout failed:", error);
            }
        });
    }
}