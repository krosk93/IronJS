/// Copyright (c) 2009 Microsoft Corporation 
/// 
/// Redistribution and use in source and binary forms, with or without modification, are permitted provided
/// that the following conditions are met: 
///    * Redistributions of source code must retain the above copyright notice, this list of conditions and
///      the following disclaimer. 
///    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and 
///      the following disclaimer in the documentation and/or other materials provided with the distribution.  
///    * Neither the name of Microsoft nor the names of its contributors may be used to
///      endorse or promote products derived from this software without specific prior written permission.
/// 
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
/// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
/// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
/// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
/// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
/// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
/// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
/// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.



ES5Harness.registerTest({

    id: "15.4.4.21-9-c-ii-5",

    path: "TestCases/chapter15/15.4/15.4.4/15.4.4.21/15.4.4.21-9-c-ii-5.js",

    description: "Array.prototype.reduce - k values are accessed during each iteration and not prior to starting the loop on an Array",

    test: function testcase() {

        var result = true;
        var kIndex = [];
        var called = 0;

        //By below way, we could verify that k would be setted as 0, 1, ..., length - 1 in order, and each value will be setted one time.
        function callbackfn(prevVal, curVal, idx, obj) {
            //Each position should be visited one time, which means k is accessed one time during iterations.
            called++;
            if (typeof kIndex[idx] === "undefined") {
                //when current position is visited, its previous index should has been visited.
                if (idx !== 0 && typeof kIndex[idx - 1] === "undefined") {
                    result = false;
                }
                kIndex[idx] = 1;
            } else {
                result = false;
            }
        }

        [11, 12, 13, 14].reduce(callbackfn, 1);

        return result && called === 4;
    },


    precondition: function prereq() {
        return fnExists(Array.prototype.reduce);
    }

});
