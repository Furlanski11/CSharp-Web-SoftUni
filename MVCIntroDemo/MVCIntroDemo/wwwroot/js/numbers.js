function SetLimit() {
	let num = document.getElementById("limitInput").value;

	window.location = "https://localhost:7186/Home/NumbersToN?count=" + num;
}