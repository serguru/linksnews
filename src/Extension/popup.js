//let siteUrl = "https://localhost:44397/";
let siteUrl = "https://linksnews.org/";
let $loginForm;
let $loginInput;
let $passwordInput;
let $addLinkForm;
let $loginError;
let $addLinkMessage;
let $generalMessageDiv;
let $pageSelect;
let $columnSelect;
let $linkNameInput;
let $linkAddressInput;
let pages = [];

function showGeneralMessage(message, error) {
    $loginForm.hide();
    $addLinkForm.hide();
    $generalMessageDiv.show();
    
    if (error) {
        $generalMessageDiv.css("color","red");
    } else {
        $generalMessageDiv.css("color","");
    }

    $generalMessageDiv.html(message);
}

function showLoginError(message) {
    $loginError.html(message);
    $loginError.show();
}

function showAddLinkMessage(message, error) {
    $addLinkMessage.html(message);
    if (error) {
        $addLinkMessage.css("color","red");
    } else {
        $addLinkMessage.css("color","");
    }
    $addLinkMessage.show();
}

function showLoading() {
    $loginForm.hide();
    $addLinkForm.hide();
    $generalMessageDiv.show();
    $generalMessageDiv.css("color","");
    $generalMessageDiv.html("Loading...");
}

function showLoginForm() {
    $generalMessageDiv.hide();
    $addLinkForm.hide();
    $loginError.hide();
    $loginForm.show();
}

function showAddLinkForm(data) {
    pages = data;

    $loginForm.hide();
    $generalMessageDiv.hide();
    populatePagesSelect();

    chrome.tabs.query({active:true},function(tab){
        if (tab.length === 0) {
            return;
        }
        $linkNameInput.val(tab[0].title);            
        $linkAddressInput.val(tab[0].url);
    });

    $addLinkForm.show();
}

function populateColumnsSelect()
{
    let pageId = $pageSelect[0].value;
    let columns = [];

    if (pageId > 0) {
        
        for(let i = 0; i < pages.length; i++) {
            if (pages[i].id == pageId) {
                columns = pages[i].columns;
                break;
            }
        }
    }

    $columnSelect.find("option").remove();

    columns.forEach(function(x) {
        let option = new Option(x.title, x.id, x.default, x.default); 
        $columnSelect.append($(option));
    });
}

function onPagesSelectChange() {
    populateColumnsSelect();
}

function populatePagesSelect() {
    pages.forEach(function(x) {
        let option = new Option(x.title, x.id, x.default, x.default); 
        $pageSelect.append($(option));
    });
    onPagesSelectChange();
}

function processError(response) {

    if (response.code === 401) {
        showLoginForm();
        return;
    }

    showGeneralMessage(response.message, true);
}

function getMyPages() {

    $.post(siteUrl + "pages/myPagesColumns/")
    .done(function (response) {
        if (response.error) {
            processError(response);
            return;
        }

        if (response.data.length === 0) {
            showGeneralMessage("You have no pages / columns to add a link to", true);
            return;
        }

        showAddLinkForm(response.data);
    })
    .fail(function (response) {
        showGeneralMessage('Error. Failed retrieving pages list.', true);
    })
    .always(function () {
        
    });

}

function login() {

    let login = $loginInput[0].value;
    let password = $passwordInput[0].value;

    if (!login || !password) {
        showLoginError("Login and Password are both required");
        return;
    }

    let data = JSON.stringify({
            login: login,
            password: password
        });

    $.post(siteUrl + "account/login/", data)
    .done(function (response) {

        if (response.error) {
            showLoginError(response.message);
            return;
        }

        getMyPages();
    })
    .fail(function (response) {
        showLoginError('Error. Failed trying to log in.');
    })
    .always(function () {
        
    });
}

function addLink() {

    let linkName = $linkNameInput[0].value;
    let linkAddress = $linkAddressInput[0].value;

    if (!linkName || !linkAddress) {
        showAddLinkMessage("Link Name and Link Address are both required", true);
        return;
    }

    if (linkName.length > 100) {
        showAddLinkMessage("Link Name cannot be longer than 100 characters", true);
        return;
    }

    let data = JSON.stringify({
            pageId: $pageSelect[0].value,
            columnId: $columnSelect[0].value,
            title: linkName,
            href: linkAddress
        });

    $.post(siteUrl + "pages/addLink/", data)
    .done(function (response) {

        if (response.error) {
            showAddLinkMessage(response.message, true);
            return;
        }
        
        showGeneralMessage("Link has been added");

        setTimeout(function(){
            window.close();
        }, 2000);
    })
    .fail(function (response) {
        showAddLinkMessage('Error. Failed adding a link.', true);
    })
    .always(function () {
        
    });
}

$(document).ready(function(){

    $.ajaxSetup({
      contentType: "application/json; charset=utf-8"
    });

    $generalMessageDiv = $("#generalMessageDiv");
    $loginForm = $("#loginForm");
    $loginInput = $("#loginInput");
    $passwordInput = $("#passwordInput");
    $addLinkForm = $("#addLinkForm");
    $linkNameInput = $("#linkNameInput");
    $linkAddressInput = $("#linkAddressInput");
    $loginError = $("#loginError");
    $addLinkMessage = $("#addLinkMessage");
    $pageSelect = $("#pageSelect");
    $columnSelect = $("#columnSelect");

    $pageSelect.on("change", function(e){
        onPagesSelectChange();
    });

    $("#loginBtn").on("click", function(){
        login();
    });

    $("#addLinkBtn").on("click", function(){
        addLink();
    });

    getMyPages();
})
