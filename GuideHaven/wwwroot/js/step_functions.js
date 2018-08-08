var index = 0, visual_index = 0;

function createStep() {
	index++;
	visual_index++;
	document.getElementById("step_holder").insertAdjacentHTML('beforeend', '<div id="step' + (index + 1) + '" style="position:relative">'
	+ '<hr/><label id="step_count_' + index + '" class="control-label" style="display: block">Step ' + (visual_index + 1) + '</label>'
	+ '<label class="control-label" for="GuideSteps_' + index + '__Header">Header</label>'
	+ '<input type="text" id="GuideSteps_' + index + '__Header" name="GuideSteps[' + index + '].Header" class="form-control">'
	+ '<label class="control-label" for="GuideSteps_' + index + '__Content">Content</label>'
	+ '<textarea id="GuideSteps_' + index + '__Content" name="GuideSteps[' + index + '].Content" class="bs-textarea"></textarea>'
	+ '<button type="button" class="btn btn-link delete-btn" value="Delete" onclick="deleteStep(' + (index + 1) + ')">'
	+ '<span class="glyphicon glyphicon-remove"></span></button></div > ');
	// console.log(index.toString());
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