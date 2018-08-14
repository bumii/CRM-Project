$(document).ready(function () {
    GetCalendarEvents();
});

function GetCalendarEvents() {

    $('#calendar').fullCalendar({
        lang: 'tr',
        locale: 'tr',

        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        editable: true,
        events: '/Calendar/GetCalendarEvents/',
        eventClick: function (event) {
            alert('Item ID: ' + event.id + "\nItem Title: " + event.title);
        }
    });
}
// Seçilen tarih için yeni event oluşturma, modal açar
function NewItem(selectedDate) {
    var html = '';

    html += '<div class="row form-horizontal">';
    html += '<div class="col-md-12">';
    html += '<div class="form-group">';
    html += '<div class="col-lg-12 control-label">Başlık (Max. 1000 Karakter)</div>';
    html += '<div class="col-lg-12">';
    html += '<input type="text" class="form-control" id="txtTitle" />';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';

    html += '<div class="row form-horizontal">';
    html += '<div class="col-md-12">';
    html += '<div class="form-group">';
    html += '<div class="col-lg-12 control-label">Not (Max. 4000 Karakter)</div>';
    html += '<div class="col-lg-12">';
    html += '<textarea type="text" class="form-control" id="txtNote">  </textarea>';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';


    html += '<div class="row form-horizontal">';
    html += '<div class="col-md-12">';
    html += '<div class="form-group">';
    html += '<div class="col-lg-12 control-label">Başlangıç Tarihi</div>';
    html += '<div class="col-lg-12">';
    html += '<input type="text" class="form-control" id="txtStartDate" value="' + selectedDate + '" />';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';

    html += '<div class="row form-horizontal">';
    html += '<div class="col-md-12">';
    html += '<div class="form-group">';
    html += '<div class="col-lg-12 control-label">Bitiş Tarihi</div>';
    html += '<div class="col-lg-12">';
    html += '<input type="text" class="form-control" id="txtEndDate" value="' + selectedDate + '" />';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';

    html += '<div class="row form-horizontal">';
    html += '<div class="col-md-12">';
    html += '<div class="form-group">';
    html += '<div class="col-lg-12 control-label">Renk</div>';
    html += '<div class="col-lg-12">';
    html += '<select class="form-control" id="ddColor">';
    html += '<option value="red">Kırmızı</option>';
    html += '<option value="blue">Mavi</option>';
    html += '<option value="green">Yeşil</option>';
    html += '</select>';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';


    html += '<div class="row form-horizontal">';
    html += '<div class="col-md-12">';
    html += '<div class="form-group">';
    html += '<div class="col-lg-12 control-label"></div>';
    html += '<div class="col-lg-12">';
    html += '<div class="form-check">';
    html += '<label class="form-check-label" for= "defaultCheck1">';
    html += 'Tüm Gün Boyunca  &emsp\x3b ';
    html += '</label>';
    html += '<input type="checkbox" id="chbox" checked="checked">';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';


    bootbox.dialog({
        message: html,
        title: "Yeni",
        size: "large",
        buttons: {
            success: {
                label: "Kaydet",
                className: "btn-success",
                callback: function () {
                    var item = {
                        id: 0,
                        title: $('#txtTitle').val(),
                        note: $('#txtNote').val(),
                        color: $('#ddColor').val(),
                        start: $('#txtStartDate').val(),
                        end: $('#txtEndDate').val(),
                        allDay: $('#chbox').is(":checked"),

                    }

                    SaveItem(item);
                }
            },
            danger: {
                label: "İptal",
                className: "btn-default"
            }
        }
    });
}
// modal'da girilen verileri kaydeder
function SaveItem(item) {
    $.ajax({
        type: "POST",
        url: "/Calendar/AddOrEditItem/",
        data: "{\"item\":" + JSON.stringify(item) + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        error: function (request, status, error) {
            var obj = jQuery.parseJSON(request.responseText);
            bootbox.alert(obj.Message);
        },
        success: function (msg) {
            $('#calendar').fullCalendar('refetchEvents');
            $('#calendar').fullCalendar('unselect');
        }
    });
}
// sürükle bırak veya resize ile tarih bilgilerini güncelleme
function UpdateItemDate(selectedItem) {
    debugger;

    var StartDate = selectedItem.start.format();
    var EndTime = selectedItem.end != null ? selectedItem.end.format() : selectedItem.start.format();

    $.ajax({
        type: "POST",
        url: "/Calendar/UpdateItemDate/",
        data: "{\"id\":" + selectedItem.id + ", \"start\":'" + StartDate + "', \"end\":'" + EndTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        error: function (request, status, error) {
            debugger;
            var obj = jQuery.parseJSON(request.responseText);
            bootbox.alert(obj.Message);
        },
        success: function (msg) {

        }
    });
}

function bootBoxModal(title, message, id) {
    bootbox.dialog({
        closeButton: false,
        message: message,
        title: title,
        buttons: {
            success: {
                label: "Sil",
                className: "btn-danger",
                callback: function () {
                    FnDelete(id);
                }
            },
            danger: {
                label: "Kapat",
                className: "btn-default"
            }
        }
    });
}
// tıklanan event için silme onayı
function DeleteItem(id) {
    var msg_ = "<h4> Not :</h4> ";
    var title_ = "";
    $.ajax({
        type: "GET",
        url: "/Calendar/GetNote/",
        data: "id=" + id,
        dataType: "json",
        success: function (result) {
            title_ += result[0].title;
            msg_ += result[0].note;
            console.log(title_, msg_);
            bootBoxModal(title_, msg_, id);
        }
    });

}

// tıklanan event'i siler
function FnDelete(id) {
    jQuery.ajax({
        type: "POST",
        url: "/Calendar/DeleteItem/",
        data: "{'id':'" + id + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        error: function (request, status, error) {
            var obj = jQuery.parseJSON(request.responseText);
            bootbox.alert(obj.Message);
        },
        success: function (msg) {
            if (msg) {

                $('#calendar').fullCalendar('refetchEvents');
                $('#calendar').fullCalendar('unselect');

                bootbox.alert("Silme işlemi başarılı");
            }
        }
    });
}
$('#calendar').fullCalendar({
    height: 650,
    locale: 'tr',

    //aspectRatio: 3,
    lang: 'tr',
    header: {
        left: 'prev,next today',
        center: 'title',
        right: 'month,agendaWeek,agendaDay'
    },
    editable: true,
    events: '/Calendar/GetCalendarEvents/',
    dayClick: function (date, jsEvent, view) {
        NewItem(date.format());
    },
    eventClick: function (event) {
        DeleteItem(event.id);
    },
    eventDrop: function (event, delta, revertFunc) {
        UpdateItemDate(event);
    },
    eventResize: function (event, delta, revertFunc) {
        UpdateItemDate(event);
    }

});