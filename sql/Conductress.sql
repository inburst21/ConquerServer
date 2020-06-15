DELETE FROM cq_action WHERE id>=1000 AND id<2000 LIMIT 1000;
DELETE FROM cq_task WHERE id>=1000 AND id<2000 LIMIT 1000;

# To Phoenix Castle		1002 958 555
# To Ape City			1002 555 957
# To Desert City		1002 69 473
# To Mine Cave			1002 55 398
# To bird island		1002 232 190

INSERT INTO `cq_action` VALUES ('1000', '1001', '1020', '1073', '0', 'stc(1,0) == 0');
INSERT INTO `cq_action` VALUES ('1001', '1002', '0000', '0101', '0000', 'Hello,~I~am~the~conductress.~I~can~help~you~teleport~to~the~portals~of~the~cities.~You~can~find~me~in~the~5~main~cities:~Twin~Ci');
INSERT INTO `cq_action` VALUES ('1002', '1003', '0000', '0101', '0000', 'ty,~Phoenix~Castle,~Ape~City,~Desert~City~and~Bird~Island.~Keep~in~mind~that~only~from~Twin~City~you~can~teleport~to~other~Citie');
INSERT INTO `cq_action` VALUES ('1003', '1004', '0000', '0101', '0000', 's,~from~all~other~you~can~teleport~back~to~Twin~City~and~from~all~you~can~go~to~the~market.');
INSERT INTO `cq_action` VALUES ('1004', '610014', '0000', '0102', '0000', 'Tell~me~more. 1010');

INSERT INTO `cq_action` VALUES ('1010', '1011', '0000', '0101', '0000', 'I`ll~charge~you~100~silvers~to~help~you~reaching~that~locations,~but~keep~in~mind~that~if~you`re~a~criminal~and~have~100~or~more');
INSERT INTO `cq_action` VALUES ('1011', '1012', '0000', '0101', '0000', '~PK~Points~I`ll~charge~you~10,000~silvers!~But,~that`s~all.~Please,~take~this~experience~as~a~reward~for~learning~about~me.');
INSERT INTO `cq_action` VALUES ('1012', '610014', '0000', '0102', '0000', 'Thank~you. 1013');
INSERT INTO `cq_action` VALUES ('1013', '1014', '0000', '1074', '0000', 'stc(1,0) = 1 0');
INSERT INTO `cq_action` VALUES ('1014', '1015', '0000', '1086', '0000', '5 5');
INSERT INTO `cq_action` VALUES ('1015', '1016', '0000', '1001', '0000', 'exp += %iter_var_data5 nocontribute');
INSERT INTO `cq_action` VALUES ('1016', '1020', '0000', '1027', '0000', 'self angelwing');

INSERT INTO `cq_action` VALUES ('1020', '1021', '1030', '1001', '0000', 'pk < 100');
INSERT INTO `cq_action` VALUES ('1021', '1022', '0000', '0101', '0000', 'Hello, %user_name. Where do you want to go? Remember it will cost you 100 silvers.');
INSERT INTO `cq_action` VALUES ('1022', '1023', '0000', '0102', '0000', 'Phoenix~Castle. 1040');
INSERT INTO `cq_action` VALUES ('1023', '1024', '0000', '0102', '0000', 'Ape~City. 1045');
INSERT INTO `cq_action` VALUES ('1024', '1025', '0000', '0102', '0000', 'Desert~City. 1050');
INSERT INTO `cq_action` VALUES ('1025', '1026', '0000', '0102', '0000', 'Bird~Island. 1055');
INSERT INTO `cq_action` VALUES ('1026', '1027', '0000', '0102', '0000', 'Mine~Cave. 1060');
INSERT INTO `cq_action` VALUES ('1027', '1028', '0000', '0102', '0000', 'Market. 1065');
INSERT INTO `cq_action` VALUES ('1028', '610014', '0000', '0102', '0000', 'Just~passing~by. 0');

INSERT INTO `cq_action` VALUES ('1030', '1031', '0000', '0101', '0000', 'Hello,~%user_name.~Oh,~no!~You~are~a~criminal!~As~you~are~now~wanted~by~the~police,~I~wont~say~I~will~not~teleport~you,~but~this');
INSERT INTO `cq_action` VALUES ('1031', '1022', '0000', '0101', '0000', '~will~cost~you~10,000~silvers~so~I~can~do~it.');

INSERT INTO `cq_action` VALUES ('1900', '1901', '0000', '0101', '0000', 'You doesn`t have enough money.');
INSERT INTO `cq_action` VALUES ('1901', '610014', '0000', '0102', '0000', 'Damn~it! 0');

INSERT INTO `cq_action` VALUES ('1040', '1041', '1042', '1001', '0000', 'pk < 100');
INSERT INTO `cq_action` VALUES ('1041', '1043', '1900', '1001', '0000', 'money += -100');
INSERT INTO `cq_action` VALUES ('1042', '1043', '1900', '1001', '0000', 'money += -10000');
INSERT INTO `cq_action` VALUES ('1043', '0000', '0000', '1003', '0000', '1002 958 555');

INSERT INTO `cq_action` VALUES ('1045', '1046', '1047', '1001', '0000', 'pk < 100');
INSERT INTO `cq_action` VALUES ('1046', '1048', '1900', '1001', '0000', 'money += -100');
INSERT INTO `cq_action` VALUES ('1047', '1048', '1900', '1001', '0000', 'money += -10000');
INSERT INTO `cq_action` VALUES ('1048', '0000', '0000', '1003', '0000', '1002 555 957');

INSERT INTO `cq_action` VALUES ('1050', '1051', '1052', '1001', '0000', 'pk < 100');
INSERT INTO `cq_action` VALUES ('1051', '1053', '1900', '1001', '0000', 'money += -100');
INSERT INTO `cq_action` VALUES ('1052', '1053', '1900', '1001', '0000', 'money += -10000');
INSERT INTO `cq_action` VALUES ('1053', '0000', '0000', '1003', '0000', '1002 69 473');

INSERT INTO `cq_action` VALUES ('1055', '1056', '1057', '1001', '0000', 'pk < 100');
INSERT INTO `cq_action` VALUES ('1056', '1058', '1900', '1001', '0000', 'money += -100');
INSERT INTO `cq_action` VALUES ('1057', '1058', '1900', '1001', '0000', 'money += -10000');
INSERT INTO `cq_action` VALUES ('1058', '0000', '0000', '1003', '0000', '1002 232 190');

INSERT INTO `cq_action` VALUES ('1060', '1061', '1042', '1001', '0000', 'pk < 100');
INSERT INTO `cq_action` VALUES ('1061', '1063', '1900', '1001', '0000', 'money += -100');
INSERT INTO `cq_action` VALUES ('1062', '1063', '1900', '1001', '0000', 'money += -10000');
INSERT INTO `cq_action` VALUES ('1063', '0000', '0000', '1003', '0000', '1002 55 398');

INSERT INTO `cq_action` VALUES ('1065', '1066', '1067', '1001', '0000', 'pk < 100');
INSERT INTO `cq_action` VALUES ('1066', '1068', '1900', '1001', '0000', 'money += -100');
INSERT INTO `cq_action` VALUES ('1067', '1068', '1900', '1001', '0000', 'money += -10000');
INSERT INTO `cq_action` VALUES ('1068', '1069', '0000', '1004', '0000', '1002 430 378');
INSERT INTO `cq_action` VALUES ('1069', '0000', '0000', '1003', '0000', '1036 211 196');