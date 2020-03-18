
function toString(){
	return this.name;
}

function foo() {
	alert(this);
}

foo();

var test = {name:"test"};
test.prototype.toString = toString;
test.f = foo;
test.f();


