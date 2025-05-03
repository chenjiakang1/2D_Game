package com.example.userapi.controller;

import com.example.userapi.model.User;
import com.example.userapi.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

import java.sql.Timestamp;
import java.util.List;

@RestController
@RequestMapping("/user")
public class UserController {

    @Autowired
    private UserRepository userRepository;

    private final BCryptPasswordEncoder passwordEncoder = new BCryptPasswordEncoder();

    // 注册
    @PostMapping("/register")
    public String register(@RequestBody User user) {
        if (userRepository.findByUsername(user.getUsername()) != null) {
            return "Username already exists!";
        }
        user.setPassword(passwordEncoder.encode(user.getPassword()));
        user.setRegisterTime(new Timestamp(System.currentTimeMillis()));
        user.setGold(100);
        userRepository.save(user);
        return "Register success!";
    }

    // 登录
    @GetMapping("/login")
    public String login(@RequestParam String username, @RequestParam String password) {
        User u = userRepository.findByUsername(username);
        if (u == null) {
            return "User not found!";
        }
        return passwordEncoder.matches(password, u.getPassword())
                ? "Login success!"
                : "Password incorrect!";
    }

    // 查金币
    @GetMapping("/gold")
    public String gold(@RequestParam String username) {
        User u = userRepository.findByUsername(username);
        return u == null
                ? "User not found!"
                : username + " has " + u.getGold() + " gold.";
    }

    // 增金币
    @PostMapping("/gold/add")
    public String addGold(@RequestParam String username, @RequestParam Integer amount) {
        User u = userRepository.findByUsername(username);
        if (u == null) return "User not found!";
        u.setGold(u.getGold() + amount);
        userRepository.save(u);
        return "Added " + amount + " gold. New total: " + u.getGold();
    }

    // 扣金币
    @PostMapping("/gold/remove")
    public String removeGold(@RequestParam String username, @RequestParam Integer amount) {
        User u = userRepository.findByUsername(username);
        if (u == null) return "User not found!";
        if (u.getGold() < amount) return "Not enough gold!";
        u.setGold(u.getGold() - amount);
        userRepository.save(u);
        return "Removed " + amount + " gold. New total: " + u.getGold();
    }

    // 测试
    @GetMapping("/all")
    public List<User> all() {
        return userRepository.findAll();
    }
}
