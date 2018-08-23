var index = 0,
    visual_index = 0,
    imgs = "";

var widget = uploadcare.Widget('[role=uploadcare-uploader]');

widget.onChange(function (file) {
    if (file) {
        file.done(function (info) {
            $("#image-url").val(info.cdnUrl);
            $("#desc-img").addClass("desc-img");
            $("#desc-img").attr("src", info.cdnUrl);
        });
    }
    else {
        $("#image-url").val("");
        $("#desc-img").attr("src", "");
    }
});

function uploadHandler(info, index) {
    var arr = [];
    arr.push(info.uuid);
    for (var i = 0; i < info.count; i++) {
        arr.push(info.cdnUrl + "nth/" + i + "/");
    }
    $("#images" + index).val(arr.join());
    console.log('set ' + index);
}

function widgetCleared(index) {
    $("#images" + index).val("");
}
multiWidget = uploadcare.MultipleWidget($("#multi0"));
multiWidget.onChange(function (info) {
    if (!info) widgetCleared(0);
});
multiWidget.onUploadComplete(function (info) {
    uploadHandler(info, 0);
});

function createStep(step, header, content, images) {
	index++;
    visual_index++;
    document.getElementById("step_holder").insertAdjacentHTML(
        'beforeend', '<div id="step' + (index + 1) + '" style="position:relative">'
        + '<hr/><label id="step_count_' + index + '" class="control-label" style="display: block">' + step + ' ' + (visual_index + 1) + '</label>'
        + '<label class="control-label" for="GuideSteps_' + index + '__Header">' + header + '</label>'
        + '<input type="text" id="GuideSteps_' + index + '__Header" name="GuideSteps[' + index + '].Header" class="form-control">'
        + '<label class="control-label" for="GuideSteps_' + index + '__Content">' + content + '</label>'

        + '<textarea id="DummyArea' + index + '" class="bs-textarea mdhtmlform-md" data-mdhtmlform-group="' + index + '" rows="5"></textarea>'

        + '<label class="control-label" style="margin-right: 3px;">' + images + '</label>'
        + '<input id="images' + index + '" hidden name="GuideSteps[' + index + '].Images" />'
        + '<input type="hidden" id="multi' + index + '" name="content" data-images-only data-multiple />'

        + '<textarea id="GuideSteps_' + index + '__Content" name="GuideSteps[' + index + '].Content" '
        + 'class="mdhtmlform-html" data-mdhtmlform-group="' + index + '" style="display: none"></textarea>'

        + '<button type="button" class="btn-link delete-btn" value="Delete" onclick="deleteStep(' + (index + 1) + ')">'
        + '<span class="glyphicon glyphicon-remove"></span></button></div >'
    );
    document.getElementById("step" + (index + 1)).scrollIntoView();
    new MdHtmlForm(document.getElementById('DummyArea' + index));

    multiWidget = uploadcare.MultipleWidget($("#multi" + index));
    multiWidget.onChange(function (info) {
        if (!info) widgetCleared(index);
    });
    multiWidget.onUploadComplete(function (info) {
        uploadHandler(info, index);
    });

}

function deleteStep(id) {
	let step = 2;
	document.getElementById('step_count_' + (id - 1)).remove();
	document.getElementById('GuideSteps_' + (id - 1) + '__Header').value = "";
	document.getElementById('GuideSteps_' + (id - 1) + '__Content').value = "";
	document.getElementById("step" + id).style.display = "none";
	for (var i = 1; i <= index; i++) {
		let obj;
		if ((obj = document.getElementById("step_count_" + i)) !== null) {
			obj.innerHTML = "Step " + step++;
		}
	}
	visual_index--;
}

function update_indexes(i) {
	index = visual_index = i;
	document.getElementById(i).id = "step" + (index + 1);
	document.getElementById(i + "+").id = "step_count_" + index;
}

function decodeHtml(html) {
    var txt = document.createElement("textarea");
    txt.innerHTML = html;
    return txt.value;
}
